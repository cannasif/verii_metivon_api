using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.UnitOfWork;
using verii_metivon_api.Modules.EDocuments.Application.Parameters;
using verii_metivon_api.Modules.EDocuments.Domain.Entities;

namespace verii_metivon_api.Modules.EDocuments.Application.Services;

public sealed class EDocumentQuery:PagedQuery{public ElectronicDocumentType?DocumentType{get;init;}public ElectronicDocumentStatus?Status{get;init;}public long?BranchId{get;init;}}
public sealed record EDocumentRow(long Id,Guid Uuid,string DocumentNumber,string DocumentType,string Direction,DateTime IssueDate,string Scenario,string Status,int AttemptCount,string?ProviderDocumentId,string?LastErrorMessage);
public sealed record CreateEDocumentRequest(ElectronicDocumentType DocumentType,long BranchId,string DocumentNumber,DateTime IssueDate,string Scenario,string SourceType,long SourceId,string PayloadXml);
public sealed record SaveProviderConfigurationRequest(long BranchId,EDocumentProviderType ProviderType,string Environment,string InvoiceServiceUrl,string DespatchServiceUrl,string CredentialReference,int TimeoutSeconds,int MaximumRetryCount,bool IsActive);
public interface IEDocumentService{Task<ApiResponse<PagedResult<EDocumentRow>>>GetPagedAsync(EDocumentQuery q,CancellationToken ct);Task<ApiResponse<object>>CreateAsync(CreateEDocumentRequest r,CancellationToken ct);Task<ApiResponse<object>>QueueAsync(long id,CancellationToken ct);Task<ApiResponse<object>>SaveProviderAsync(SaveProviderConfigurationRequest r,CancellationToken ct);}

public sealed class EDocumentService(IUnitOfWork u,IEDocumentParameterService parameters):IEDocumentService
{
 public async Task<ApiResponse<PagedResult<EDocumentRow>>>GetPagedAsync(EDocumentQuery q,CancellationToken ct)
 {
  var x=u.Repository<ElectronicDocument>().Query();if(q.DocumentType.HasValue)x=x.Where(v=>v.DocumentType==q.DocumentType);if(q.Status.HasValue)x=x.Where(v=>v.Status==q.Status);if(q.BranchId.HasValue)x=x.Where(v=>v.BranchId==q.BranchId);if(!string.IsNullOrWhiteSpace(q.Search)){var s=q.Search.Trim();x=x.Where(v=>v.DocumentNumber.Contains(s)||(v.ProviderDocumentId!=null&&v.ProviderDocumentId.Contains(s)));}x=x.ApplyPagedFilters(q);var total=await x.CountAsync(ct);var rows=await x.OrderByDescending(v=>v.IssueDate).ThenByDescending(v=>v.Id).Skip((q.NormalizedPageNumber-1)*q.NormalizedPageSize).Take(q.NormalizedPageSize).Select(v=>new EDocumentRow(v.Id,v.Uuid,v.DocumentNumber,v.DocumentType.ToString(),v.Direction.ToString(),v.IssueDate,v.Scenario,v.Status.ToString(),v.AttemptCount,v.ProviderDocumentId,v.LastErrorMessage)).ToListAsync(ct);return ApiResponse<PagedResult<EDocumentRow>>.Ok(new(rows,q.NormalizedPageNumber,q.NormalizedPageSize,total));
 }

 public async Task<ApiResponse<object>>CreateAsync(CreateEDocumentRequest r,CancellationToken ct)
 {
  if(!Enum.IsDefined(r.DocumentType)||r.BranchId<=0||string.IsNullOrWhiteSpace(r.DocumentNumber)||string.IsNullOrWhiteSpace(r.SourceType)||r.SourceId<=0)return ApiResponse<object>.Error("Document type, branch, number and source are required.",400);
  var settings=await parameters.ResolveSettingsAsync(r.BranchId,ct);var validation=await ValidateAsync(r.BranchId,r.DocumentType,r.PayloadXml,settings,ct);if(validation is not null)return ApiResponse<object>.Error(validation,400);
  if(settings.PreventDuplicateSourceDocument&&await u.Repository<ElectronicDocument>().ExistsAsync(x=>x.SourceType==r.SourceType&&x.SourceId==r.SourceId&&x.DocumentType==r.DocumentType,ct))return ApiResponse<object>.Error("An electronic document already exists for this source document.",409);
  var scenario=DefaultScenario(settings,r.DocumentType);if(!string.IsNullOrWhiteSpace(r.Scenario)){var requested=r.Scenario.Trim().ToUpperInvariant();if(!settings.AllowManualScenario&&requested!=scenario)return ApiResponse<object>.Error("Manual electronic document scenarios are disabled.",400);scenario=requested;}
  EDocumentProviderConfiguration? provider=null;if(settings.AutoQueueAfterCreate){provider=await parameters.ResolveProviderAsync(r.BranchId,ct);if(provider is null)return ApiResponse<object>.Error("Automatic queueing requires an active provider configuration.",409);}
  var payload=r.PayloadXml.Trim();var hash=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload)));var status=settings.AutoQueueAfterCreate?ElectronicDocumentStatus.Queued:ElectronicDocumentStatus.Ready;
  var e=new ElectronicDocument{DocumentType=r.DocumentType,Direction=ElectronicDocumentDirection.Outgoing,Status=status,BranchId=r.BranchId,DocumentNumber=r.DocumentNumber.Trim().ToUpperInvariant(),IssueDate=r.IssueDate,Scenario=scenario,SourceType=r.SourceType.Trim(),SourceId=r.SourceId,PayloadXml=payload,PayloadHash=hash,NextAttemptAt=settings.AutoQueueAfterCreate?DateTime.UtcNow:null};if(settings.AutoQueueAfterCreate)e.History.Add(new ElectronicDocumentStatusHistory{FromStatus=ElectronicDocumentStatus.Draft,ToStatus=status,Description="Automatically queued by electronic document parameters."});await u.Repository<ElectronicDocument>().AddAsync(e,ct);await u.SaveChangesAsync(ct);return ApiResponse<object>.Ok(new{e.Id,e.Uuid,e.Status,e.Scenario});
 }

 public async Task<ApiResponse<object>>QueueAsync(long id,CancellationToken ct)
 {
  var e=await u.Repository<ElectronicDocument>().GetByIdForUpdateAsync(id,ct);if(e is null)return ApiResponse<object>.Error("Electronic document not found.",404);if(e.Status is not(ElectronicDocumentStatus.Ready or ElectronicDocumentStatus.Failed))return ApiResponse<object>.Error("Document is not queueable.",409);var settings=await parameters.ResolveSettingsAsync(e.BranchId,ct);var validation=await ValidateAsync(e.BranchId,e.DocumentType,e.PayloadXml,settings,ct);if(validation is not null)return ApiResponse<object>.Error(validation,400);var config=await parameters.ResolveProviderAsync(e.BranchId,ct);if(config is null)return ApiResponse<object>.Error("Active CRS Soft provider configuration is required.",409);if(e.AttemptCount>=config.MaximumRetryCount&&config.MaximumRetryCount>0)return ApiResponse<object>.Error("Maximum provider retry count has been reached.",409);var from=e.Status;e.Status=ElectronicDocumentStatus.Queued;e.NextAttemptAt=DateTime.UtcNow.AddMinutes(from==ElectronicDocumentStatus.Failed?settings.RetryDelayMinutes:0);e.LastErrorCode=null;e.LastErrorMessage=null;await u.Repository<ElectronicDocumentStatusHistory>().AddAsync(new(){ElectronicDocumentId=e.Id,FromStatus=from,ToStatus=e.Status,Description="Queued for provider submission."},ct);await u.SaveChangesAsync(ct);return ApiResponse<object>.Ok(new{e.Id,e.Status,e.NextAttemptAt});
 }

 public async Task<ApiResponse<object>>SaveProviderAsync(SaveProviderConfigurationRequest r,CancellationToken ct)
 {
  if(!Uri.TryCreate(r.InvoiceServiceUrl,UriKind.Absolute,out _)||!Uri.TryCreate(r.DespatchServiceUrl,UriKind.Absolute,out _)||r.TimeoutSeconds is <5 or >300||r.MaximumRetryCount is <0 or >20)return ApiResponse<object>.Error("Valid service URLs, timeout and retry values are required.",400);var environment=string.Equals(r.Environment,"Production",StringComparison.OrdinalIgnoreCase)?"Production":"Test";var e=await u.Repository<EDocumentProviderConfiguration>().FirstOrDefaultAsync(x=>x.BranchId==r.BranchId&&x.ProviderType==r.ProviderType&&x.Environment==environment,true,ct);if(e is null){e=new EDocumentProviderConfiguration{BranchId=r.BranchId,ProviderType=r.ProviderType,Environment=environment};await u.Repository<EDocumentProviderConfiguration>().AddAsync(e,ct);}e.InvoiceServiceUrl=r.InvoiceServiceUrl.Trim();e.DespatchServiceUrl=r.DespatchServiceUrl.Trim();e.CredentialReference=r.CredentialReference.Trim();e.TimeoutSeconds=r.TimeoutSeconds;e.MaximumRetryCount=r.MaximumRetryCount;e.IsActive=r.IsActive;await u.SaveChangesAsync(ct);return ApiResponse<object>.Ok(new{e.Id});
 }

 async Task<string?>ValidateAsync(long branchId,ElectronicDocumentType type,string?payload,EDocumentParameterSettings p,CancellationToken ct)
 {
  if(string.IsNullOrWhiteSpace(payload))return "UBL payload is required.";if(Encoding.UTF8.GetByteCount(payload)>p.MaximumXmlSizeKb*1024)return $"UBL payload cannot exceed {p.MaximumXmlSizeKb} KB.";if(p.RequireUblValidation){var expected=type==ElectronicDocumentType.EDespatch?"DespatchAdvice":"Invoice";if(!payload.Contains($"<{expected}",StringComparison.OrdinalIgnoreCase)&&!payload.Contains($":{expected}",StringComparison.OrdinalIgnoreCase))return $"UBL payload must contain a {expected} root element.";}
  if(p.RequireCompanyLegalProfile||p.RequireTaxNumberValidation){var profile=await u.Repository<CompanyLegalProfile>().FirstOrDefaultAsync(x=>x.BranchId==branchId&&x.IsActive,false,ct);if(profile is null&&p.RequireCompanyLegalProfile)return "An active company legal profile is required.";if(profile is not null&&p.RequireTaxNumberValidation&&profile.TaxNumber.Length is not(10 or 11))return "Company tax or identity number must contain 10 or 11 digits.";}
  return null;
 }
 static string DefaultScenario(EDocumentParameterSettings p,ElectronicDocumentType type)=>type switch{ElectronicDocumentType.EInvoice=>p.DefaultInvoiceScenario,ElectronicDocumentType.EArchiveInvoice=>p.DefaultArchiveInvoiceScenario,ElectronicDocumentType.EDespatch=>p.DefaultDespatchScenario,_=>p.DefaultInvoiceScenario};
}


