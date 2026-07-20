# V3RII Senior Grade Proje Kuralları

Sürüm: 1.0  
Durum: Zorunlu mimari sözleşme  
Kapsam: V3RII .NET API, React Web ve bunların modülleri

Bu belge tek kanonik V3RII proje sözleşmesidir. CRM'de olgunlaşan iyi pratikleri ürün ailesine taşır; CRM'e özgü iş kurallarını taşımaz. Yeni proje bu dosyayla başlar. Mevcut proje ise yeni veya değiştirilen her modülü bu sözleşmeye yaklaştırır.

## 1. Değişmez ilkeler

- Mimari hedef **modüler monolit** API ve **feature-first** web yapısıdır.
- Modül kendi domain, uygulama, API, altyapı, eşleme, doğrulama, localization ve test varlıklarının sahibidir.
- Modüller birbirinin iç klasörlerine erişmez; yalnız yayımlanmış contract veya application arayüzünü kullanır.
- Controller/component iş kuralı içermez. Entity API response'u veya web form modeli değildir.
- Kullanıcıya görünen metin hard-coded olamaz. Yetkilendirme varsayılan olarak kapalıdır.
- Listeleme, arama, filtre, sıralama ve sayfalama sunucuda yapılır.
- Birden fazla kalıcı yazma tek iş akışına aitse transaction zorunludur.
- Yeni bağımlılık eklemek senior-grade olmanın kanıtı değildir; kullanım sınırı bu belgede tanımlı olmalıdır.

**MUST** kalite kapısıdır. **SHOULD** kuralından sapma ADR ile açıklanır. **MAY** ihtiyaca bağlıdır.

## 2. API modül sözleşmesi

```text
Modules/<ModuleName>/
  Api/
    Controllers/
    Contracts/
  Application/
    Abstractions/
    Commands/<UseCase>/
    Queries/<UseCase>/
    Dtos/
    Mappings/
    Services/
    Validators/
  Domain/
    Entities/
    ValueObjects/
    Enums/
    Events/
    Rules/
  Infrastructure/
    Persistence/Configurations/
    Persistence/Repositories/
    Integrations/
    Jobs/
  Localization/
    <ModuleName>LocalizationResource.cs
    Resources/<culture>.json
  ModuleRegistration.cs
  ModuleManifest.cs
```

Boş klasör açılmaz; dosya oluştuğunda doğru sınırın dışında tutulamaz. `ModuleManifest` modül anahtarı/sürümü, modül bağımlılıkları, permission anahtarları, localization resource, health check ve endpoint grubunu yayımlar. `ModuleRegistration` DI, validator, mapping profile, options ve infrastructure kayıtlarının tek girişidir. `Program.cs` tek tek service bilmez.

### 2.1 Bağımlılık yönü

```text
Api -> Application -> Domain
Infrastructure -> Application + Domain
Host -> ModuleRegistration
Domain -> hiçbir dış katman
```

- Domain EF Core, HTTP, localization, JWT, dosya sistemi veya UI bilmez.
- Application yalnız abstraction üzerinden dış sisteme erişir; Infrastructure bunu uygular.
- Module A, Module B entity/repository'sine doğrudan erişemez. Contract, domain event veya açık application service kullanır.
- Döngüsel modül bağımlılığı yasaktır.

### 2.2 Use-case ve controller

- Controller yalnız HTTP contract, authentication/authorization, model binding ve status code yönetir.
- Her yazma işlemi niyetini anlatan command/request kullanır; entity bind edilmez.
- Her okuma query/read model döndürür. CancellationToken tüm async zincirde iletilir.
- Controller içinde `DbContext`, repository, transaction, entity mapping veya iş kuralı MUST NOT bulunur.

### 2.3 DTO ve AutoMapper politikası

AutoMapper standardın bir aracıdır; her eşleme otomatik yapılmaz.

- Basit, birebir ve hesap içermeyen entity -> detail DTO eşlemesi `Application/Mappings` profile'ı ile MAY yapılır.
- EF liste sorgusu `Select` projection veya AutoMapper `ProjectTo` ile SQL'e çevrilmelidir; entity listesini belleğe alıp sonra map etmek yasaktır.
- Finansal hesap, permission ile alan gizleme, localization, aggregate birleştirme veya özel performans gerektiren read model açık `Select` ile MUST yazılır.
- Create/update request -> mevcut entity için kör `Map` yasaktır. Mass-assignment riskine karşı değiştirilebilir alanlar açıkça atanır.
- Navigation collection'ları varsayılan otomatik map edilmez.
- Her profile için `AssertConfigurationIsValid` testi bulunur; mapping hatası runtime'da keşfedilmez.

### 2.4 Doğrulama ve domain kuralları

- HTTP alan kuralları `Application/Validators` altında FluentValidation ile yapılır.
- Değişmez iş kuralları entity/value object/domain rule içinde korunur.
- Benzersizlik ve ilişki bütünlüğü DB constraint/index ile de korunur.
- DB kullanan validator async çalışır ve CancellationToken alır.
- Hata kodu sabit, kullanıcı mesajı localization anahtarıdır.

### 2.5 Persistence, repository ve Unit of Work

- Write use-case UoW üzerinden transaction sınırını yönetir; çoklu yazmada explicit transaction MUST kullanır.
- Read query `AsNoTracking` ve projection kullanır.
- Generic repository basit aggregate CRUD içindir; karmaşık sorgu query service veya modül repository'sine taşınır.
- `IQueryable` API/controller'a sızmaz. `SaveChanges` döngü içinde çağrılmaz.
- Optimistic concurrency gereken kayıtta `rowversion`/concurrency token bulunur.
- Silme işlemi tüm `Entity` kayıtlarında istisnasız soft-delete'tir: `IsDeleted = true`, `DeletedAt` ve mümkünse `DeletedBy` yazılır; fiziksel `DELETE` üretilmez.
- Normal tüm EF sorguları merkezi global query filter ile yalnızca `IsDeleted = false` kayıtları döndürür. `IgnoreQueryFilters` yalnız açıkça belgelenmiş yönetim, geri yükleme veya seed senaryosunda kullanılabilir.
- Soft-delete edilen kayıt benzersiz iş anahtarını bloke etmez; unique indeksler `[IsDeleted] = 0` koşuluyla filtrelenir.
- HTTP `DELETE` veya IIS uyumlu `/delete` endpoint'i yalnız taşıma sözleşmesidir; veritabanı semantiğini hard-delete'e çeviremez.
- Migration niyetli isim taşır ve SQL etkisi incelenir.
- Production başlangıcında kontrolsüz otomatik migration SHOULD NOT çalışır.

### 2.6 Transaction, idempotency ve güvenilir mesajlaşma

- Harici servis çağrısı DB transaction'ını uzun süre açık tutmaz.
- DB değişikliği sonrası entegrasyon mesajında Outbox pattern SHOULD kullanılır.
- Mal kabul, stok hareketi, sevk, e-belge ve ödeme gibi kritik command'larda idempotency key MUST bulunur.
- Retry yalnız transient hata için sınırlı backoff ile yapılır; işlem anahtarı DB unique constraint ile korunur.

### 2.7 API response ve hata sözleşmesi

- Başarılı response tek tip envelope veya açık REST contract kullanır; ikisi aynı projede karışmaz.
- Hatalar merkezi exception handler ile RFC Problem Details biçimine çevrilir.
- Validation, not-found, conflict, forbidden, concurrency ve integration hata türleri sabit kod taşır.
- Stack trace, SQL, dosya yolu veya secret kullanıcıya dönmez.
- Correlation/trace ID response ve logda aynıdır; domain exception controller'larda tek tek catch edilmez.

### 2.8 Localization

- Her modül kendi localization namespace/resource'una ve 15 dilde eş key şemasına sahiptir.
- API kullanıcı mesajları resource key'den gelir; kod içine fallback Türkçe gömülmez.
- Enum görünen adları, validation ve business error mesajları localization kapsamındadır.
- Log çevrilmez; yapılandırılmış İngilizce event adı kullanılır.

### 2.9 Güvenlik ve denetim

- Endpoint policy'siz yayımlanamaz; anonymous endpoint açıkça işaretlenir.
- Permission'lar manifestte tanımlanır; create/read/update/delete/export/import/approve/post ayrılabilir.
- Branch/tenant filtresi UI değerine güvenmez, authenticated context'ten doğrulanır.
- Secret git içinde tutulmaz. Kritik değişiklik audit log taşır.
- Upload boyut, MIME, uzantı, zararlı içerik ve path traversal kontrollerinden geçer.
- Login, password reset, upload ve pahalı query endpointlerinde rate limiting uygulanır.

### 2.10 Gözlemlenebilirlik

- Structured logging, correlation ID ve request duration MUST bulunur.
- OpenTelemetry HTTP, ASP.NET ve SQL trace/metric SHOULD bulunur.
- `/health/live` ile `/health/ready` ayrıdır; ready DB ve kritik bağımlılıkları ölçer.
- PII/secret loglanmaz; background job deneme sayısı, süre ve son hatayı kaydeder.

### 2.11 API test piramidi

Her modül domain rule, validator, mapping configuration, use-case, persistence integration ve auth/endpoint contract testlerini içerir. Architecture testleri şunları engeller:

- Domain'in EF/ASP.NET bağımlılığı,
- controller içinde DbContext,
- modüller arası iç namespace erişimi,
- entity'nin API dönüş tipi olması,
- localization'sız kullanıcı mesajı,
- kayıtsız validator/profile,
- policy'siz controller action.

## 3. Web feature sözleşmesi

```text
src/features/<feature-name>/
  api/
  components/
  hooks/
  pages/
  schemas/
  types/
  mappers/
  localization/<15 dil>.json
  permissions.ts
  query-keys.ts
  routes.tsx
  manifest.ts
  index.ts
  __tests__/
```

- Feature dış dünyaya yalnız `index.ts` public API'sini açar; başka feature'a deep import yasaktır.
- `manifest.ts` route, menu, permission, namespace, lazy entry ve sürümü tanımlar.
- Feature taşındığında route, localization ve permission dahil çalışabilir olmalıdır.
- Ürün genelinde ortak parçalar `src/shared`; feature'a özel parçalar shared'a taşınmaz.

### 3.1 API ve server-state

- Axios instance, token, refresh, timeout ve error normalization `shared/api` içindedir.
- Component raw axios/fetch ve endpoint string bilmez; feature typed API adapter kullanır.
- TanStack Query key response'u etkileyen page, size, search, sort, filter, branch ve dili kapsar.
- Mutation sonrası invalidate/refetch açık yazılır; AbortSignal desteklenir.
- API DTO ile form/view modeli gerektiğinde mapper ile ayrılır.

### 3.2 Form ve validation

- Formlar React Hook Form + Zod ile SHOULD kurulur; API DTO doğrudan form state değildir.
- Zorunlu alan etikette `*`, submit sonrası alan çevresinde ve açıklamasında hata gösterir.
- ID kullanıcı tarafından yazılmaz; searchable/infinite selector ile seçilir.
- Double-submit engellenir. Server validation alanlara, business hata form geneline bağlanır.

### 3.3 Liste ve grid

- Tüm liste istekleri POST JSON paged contract kullanır.
- Search, advanced filter, filter logic, sort ve paging DB'ye kadar taşınır.
- `recordId` ilk, gizlenemez sütun; actions son, gizlenemez sütundur.
- Edit/delete yalnız permission varsa görünür; delete/deactivate localization'lı confirmation ister.
- Loading, refreshing, empty, error, forbidden ve stale state'leri ayrıdır.
- Export aktif filtre/sort kapsamını korur. Satır key'i stable DB ID'dir.

### 3.4 Localization, RTL ve formatlama

- Görünen her metin feature localization dosyasındadır; `defaultValue` eksik çeviriyi gizleyemez.
- 15 dil aynı key şemasındadır; kalite scripti eksik/fazla key'i engeller.
- Arapça `dir=rtl`; layout ve icon yönleri uyarlanır.
- Dil değişiminde kısa global loading boundary gösterilir.
- Tarih, sayı, tutar ve para merkezi formatter ile genel ayarlara göre gösterilir; API/DB culture-neutral kalır.

### 3.5 UI, erişilebilirlik ve responsive

- Tema feature içinde sidebar geometrisini değiştirmez; design token kullanır.
- Mobile, tablet, laptop ve geniş ekran smoke test edilir; dialog/portal da tema tokenı kullanır.
- Klavye, focus, label, aria-name ve contrast zorunludur.
- Tooltip ağacı kökte tek `TooltipProvider` altındadır.
- Route error boundary ve lazy fallback bulunur; büyük modüller route bazlı lazy yüklenir.

### 3.6 Web testleri

Schema/mapper unit, hook/API adapter, component interaction, route permission, localization schema, responsive/RTL smoke ve kritik akış E2E testleri bulunur. Console error, unhandled rejection veya beklenmeyen failed request testi düşürür.

## 4. Ortak contract'lar

Paged request/response, filter/sort, Problem Details kodları, permission, enum, serialization, idempotency ve correlation header'ları API/web arasında tek sözleşmedir. OpenAPI'den TypeScript client/type üretimi SHOULD kullanılır; elle kopyalanmış DTO farkı CI'da engellenir.

### 4.1 Para birimi ve kur ana veri sözleşmesi

- Para birimi kullanıcı tarafından serbest metin olarak yazılamaz. Finansal veya ticari kayıt `CurrencyId` ile aktif `RII_CURRENCIES` kaydına bağlanır; web searchable/infinite selector kullanır.
- `CurrencyCode` ve `CurrencySymbol` yalnız okunabilir gösterim, entegrasyon veya tarihsel snapshot amacıyla tutulabilir. API bunları istemciden güvenilir ana veri olarak kabul etmez; seçilen `CurrencyId` üzerinden üretir.
- İşlem anındaki kur sonradan değişmeyecek şekilde `OriginalExchangeRate`, `AppliedExchangeRate`, `ExchangeRateDate` ve `ExchangeRateSource` ile snapshot alınır. Kaynak kur salt okunur; yetkili kullanıcı uygulanan kuru değiştirebilir.
- Harici kur birden fazla para birimi birimi için yayımlanıyorsa birim kur `quoted rate / unit` olarak normalize edilir. Satış kuru yoksa tanımlı fallback politikası (örneğin kıymetli madenlerde alış kuru) açıkça uygulanır.
- Sistem ana para biriminin kuru 1'dir. Ana para birimi kodu hard-code edilmez; Genel Ayarlar'daki `DefaultCurrencyId` belirleyicidir.
- Geçmiş işlem kayıtları güncel kur servisi yeniden çağrılarak hesaplanmaz. Maliyet, muhasebe ve audit her zaman kayıt anındaki snapshot değerlerini kullanır.
- Migration eski serbest metin kodlarını ISO kodu/kod eşleşmesiyle `CurrencyId`'ye backfill eder; eşleşmeyen veriyi sessizce 0 ID ile ilişkilendiremez.

## 5. Kod kalitesi

### API MUST

- Nullable açık, yeni warning yok; async kodda `.Result`/`.Wait()` yok.
- Tek satıra sıkıştırılmış service/controller kabul edilmez.
- Magic değer option, enum, value object veya sabite taşınır.
- Saat için `TimeProvider`, kimlik için current-user abstraction kullanılır.

### Web MUST

- TypeScript strict; açıklamasız `any` yok.
- Render sırasında state setter, production debug console ve component içinde endpoint/permission stringi yok.
- Hook uyarısı susturulmaz; kök neden çözülür.

## 6. CI kalite kapıları

```text
API: restore -> format/analyzer -> build -> unit -> architecture
     -> integration -> migration review -> security scan -> publish -> health smoke

WEB: npm ci -> lint -> typecheck -> localization -> architecture
     -> unit/component -> build -> E2E smoke -> bundle budget -> deploy smoke
```

Build başarılı fakat test, çeviri veya architecture gate başarısızsa deploy yapılamaz.

## 7. Definition of Done

- [ ] Modül/feature sözleşmesine uygun klasör ve manifest var.
- [ ] Entity API'ye dönmüyor; DTO/read model var.
- [ ] Validator ve domain invariant'ları var.
- [ ] Mapping politikası doğru ve testli.
- [ ] Read sorguları projection + AsNoTracking kullanıyor.
- [ ] Multi-write transaction, kritik command idempotent.
- [ ] Permission, branch scope ve audit tamam.
- [ ] 15 dil key şeması eşit; RTL kontrol edildi.
- [ ] Loading/empty/error/forbidden/confirmation state'leri var.
- [ ] Server paging/filter/sort çalışıyor.
- [ ] Unit, integration, architecture ve gerekli E2E testleri geçiyor.
- [ ] Log/trace/health ve kullanıcı hata kodu var.
- [ ] API ve web build/quality komutları temiz.
- [ ] Migration ve production rollback etkisi incelendi.

## 8. Mevcut projeleri dönüştürme sırası

Big-bang yeniden yazım yapılmaz; **strangler + ratchet** uygulanır:

1. Shared error, paging, current-user, time, localization ve observability çekirdeğini sabitle.
2. Architecture test ve web contract checker ekle; mevcut ihlalleri baseline'a al, yeni ihlali yasakla.
3. AutoMapper/validator kayıtlarını module registration üzerinden kur.
4. Önce Organization/Identity, sonra Products/Warehouses, sonra hareket modüllerini dikey dilim taşı.
5. Her API modülüyle ilgili web feature'ını manifest + localization yapısına taşı.
6. Legacy global kod yalnız son tüketici taşındığında kaldırılır.
7. Her sprint baseline ihlal sayısını azaltır; artırmak yasaktır.

Metivon başlangıç ölçümü (2026-07-19): 39 controller, yaklaşık 40 service, 0 validator, 0 mapping profile, 2 localization alanı ve 0 test projesi. İlk hedef kütüphane eklemek değil; ilk üç çekirdek modülü sözleşmenin tüm dikey dilimiyle tamamlamaktır.

## 9. Mimari sapma (ADR)

Sapma gerekiyorsa sürümlenen ADR; bağlamı, kararı, alternatifleri, güvenlik/performans etkisini, geri alma koşulunu, sahibini ve son kullanma tarihini içerir. Bunlar yoksa istisna kabul edilmez.
