namespace verii_metivon_api.Modules.Products.Localization;

public static class ProductMessages
{
    private static readonly Dictionary<string, string[]> Values = new(StringComparer.OrdinalIgnoreCase)
    {
        ["tr"] = ["Ürünler başarıyla getirildi.", "Ürün başarıyla oluşturuldu.", "Ürün tanımları getirildi.", "Ürün kodu ve adı zorunludur.", "Bu ürün kodu zaten kullanılıyor.", "Ürün bilgileri geçersiz."],
        ["en"] = ["Products retrieved successfully.", "Product created successfully.", "Product definitions retrieved.", "Product code and name are required.", "Product code already exists.", "Product data is invalid."],
        ["de"] = ["Produkte erfolgreich abgerufen.", "Produkt erfolgreich erstellt.", "Produktdefinitionen abgerufen.", "Produktcode und Name sind erforderlich.", "Der Produktcode existiert bereits.", "Die Produktdaten sind ungültig."],
        ["fr"] = ["Produits récupérés avec succès.", "Produit créé avec succès.", "Définitions de produit récupérées.", "Le code et le nom sont obligatoires.", "Ce code produit existe déjà.", "Les données du produit sont invalides."],
        ["es"] = ["Productos obtenidos correctamente.", "Producto creado correctamente.", "Definiciones de producto obtenidas.", "El código y el nombre son obligatorios.", "El código del producto ya existe.", "Los datos del producto no son válidos."],
        ["it"] = ["Prodotti recuperati correttamente.", "Prodotto creato correttamente.", "Definizioni prodotto recuperate.", "Codice e nome sono obbligatori.", "Il codice prodotto esiste già.", "I dati del prodotto non sono validi."],
        ["pt"] = ["Produtos obtidos com sucesso.", "Produto criado com sucesso.", "Definições do produto obtidas.", "Código e nome são obrigatórios.", "O código do produto já existe.", "Os dados do produto são inválidos."],
        ["nl"] = ["Producten succesvol opgehaald.", "Product succesvol aangemaakt.", "Productdefinities opgehaald.", "Productcode en naam zijn verplicht.", "De productcode bestaat al.", "De productgegevens zijn ongeldig."],
        ["pl"] = ["Produkty zostały pobrane.", "Produkt został utworzony.", "Pobrano definicje produktów.", "Kod i nazwa produktu są wymagane.", "Kod produktu już istnieje.", "Dane produktu są nieprawidłowe."],
        ["ru"] = ["Товары успешно получены.", "Товар успешно создан.", "Справочники товаров получены.", "Код и название товара обязательны.", "Код товара уже существует.", "Данные товара недействительны."],
        ["ar"] = ["تم جلب المنتجات بنجاح.", "تم إنشاء المنتج بنجاح.", "تم جلب تعريفات المنتج.", "رمز المنتج واسمه مطلوبان.", "رمز المنتج موجود بالفعل.", "بيانات المنتج غير صالحة."],
        ["fa"] = ["محصولات با موفقیت دریافت شدند.", "محصول با موفقیت ایجاد شد.", "تعاریف محصول دریافت شد.", "کد و نام محصول الزامی است.", "کد محصول از قبل وجود دارد.", "داده‌های محصول نامعتبر است."],
        ["zh"] = ["产品获取成功。", "产品创建成功。", "产品定义获取成功。", "产品代码和名称为必填项。", "产品代码已存在。", "产品数据无效。"],
        ["ja"] = ["商品を取得しました。", "商品を作成しました。", "商品定義を取得しました。", "商品コードと名称は必須です。", "商品コードは既に存在します。", "商品データが無効です。"],
        ["ko"] = ["제품을 불러왔습니다.", "제품을 생성했습니다.", "제품 정의를 불러왔습니다.", "제품 코드와 이름은 필수입니다.", "제품 코드가 이미 존재합니다.", "제품 데이터가 올바르지 않습니다."]
    };
    private static readonly string[] Keys = ["Retrieved", "Created", "DefinitionsRetrieved", "Required", "DuplicateCode", "Invalid"];
    public static string Get(string key, string? culture)
    {
        var language = culture?.Split(',')[0].Split('-')[0].ToLowerInvariant() ?? "tr";
        if (!Values.TryGetValue(language, out var values)) values = Values["tr"];
        var index = Array.IndexOf(Keys, key); return index >= 0 ? values[index] : key;
    }
}
