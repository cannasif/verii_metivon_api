namespace verii_metivon_api.Modules.Products.Domain.Enums;

public enum ProductType { Goods = 1, Service = 2, RawMaterial = 3, SemiFinished = 4, FinishedProduct = 5, Consumable = 6, PackagingMaterial = 7, FixedAsset = 8 }
public enum InventoryTrackingType { None = 0, Lot = 1, Serial = 2 }
public enum InventoryValuationMethod { MovingAverage = 1, Fifo = 2, StandardCost = 3 }
public enum ProcurementType { Purchase = 1, Manufacture = 2, Both = 3 }
public enum BarcodeType { Ean8 = 1, Ean13 = 2, Upca = 3, Gtin14 = 4, Code128 = 5, Qr = 6, Internal = 7 }
public enum RoundingMethod { None = 0, Nearest = 1, Up = 2, Down = 3 }
public enum ProductLifecycleStatus { Draft = 0, Active = 1, Blocked = 2, Discontinued = 3 }
