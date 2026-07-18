namespace verii_metivon_api.Modules.BusinessPartners.Localization;

public static class BusinessPartnerMessages
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Resources =
        new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["tr"] = new Dictionary<string, string> {
                ["Retrieved"] = "Cari kayıtları başarıyla getirildi.", ["Created"] = "Cari başarıyla oluşturuldu.",
                ["DefinitionsRetrieved"] = "Cari tanımları başarıyla getirildi.", ["Required"] = "Cari kodu ve adı zorunludur.",
                ["DuplicateCode"] = "Bu cari kodu zaten kullanılıyor.", ["InvalidTckn"] = "TCKN 11 karakter olmalıdır." },
            ["en"] = new Dictionary<string, string> {
                ["Retrieved"] = "Business partners retrieved successfully.", ["Created"] = "Business partner created successfully.",
                ["DefinitionsRetrieved"] = "Business partner definitions retrieved successfully.", ["Required"] = "Code and name are required.",
                ["DuplicateCode"] = "Business partner code already exists.", ["InvalidTckn"] = "National identity number must be 11 characters." },
            ["de"] = D("Geschäftspartner erfolgreich abgerufen.", "Geschäftspartner erfolgreich erstellt.", "Definitionen erfolgreich abgerufen.", "Code und Name sind erforderlich.", "Der Geschäftspartnercode existiert bereits.", "Die nationale ID muss 11 Zeichen lang sein."),
            ["fr"] = D("Partenaires récupérés avec succès.", "Partenaire créé avec succès.", "Définitions récupérées avec succès.", "Le code et le nom sont obligatoires.", "Ce code existe déjà.", "L’identifiant national doit comporter 11 caractères."),
            ["es"] = D("Socios obtenidos correctamente.", "Socio creado correctamente.", "Definiciones obtenidas correctamente.", "El código y el nombre son obligatorios.", "El código ya existe.", "El número de identidad debe tener 11 caracteres."),
            ["it"] = D("Partner recuperati correttamente.", "Partner creato correttamente.", "Definizioni recuperate correttamente.", "Codice e nome sono obbligatori.", "Il codice esiste già.", "Il numero identificativo deve contenere 11 caratteri."),
            ["pt"] = D("Parceiros obtidos com sucesso.", "Parceiro criado com sucesso.", "Definições obtidas com sucesso.", "Código e nome são obrigatórios.", "O código já existe.", "O número de identidade deve ter 11 caracteres."),
            ["nl"] = D("Zakenpartners succesvol opgehaald.", "Zakenpartner succesvol aangemaakt.", "Definities succesvol opgehaald.", "Code en naam zijn verplicht.", "De code bestaat al.", "Het identiteitsnummer moet 11 tekens bevatten."),
            ["pl"] = D("Partnerzy zostali pobrani.", "Partner został utworzony.", "Definicje zostały pobrane.", "Kod i nazwa są wymagane.", "Kod partnera już istnieje.", "Numer identyfikacyjny musi mieć 11 znaków."),
            ["ru"] = D("Деловые партнеры успешно получены.", "Деловой партнер успешно создан.", "Справочники успешно получены.", "Код и название обязательны.", "Код партнера уже существует.", "Идентификационный номер должен содержать 11 символов."),
            ["ar"] = D("تم جلب شركاء الأعمال بنجاح.", "تم إنشاء شريك الأعمال بنجاح.", "تم جلب تعريفات الشركاء بنجاح.", "الرمز والاسم مطلوبان.", "رمز شريك الأعمال موجود بالفعل.", "يجب أن يتكون رقم الهوية من 11 خانة."),
            ["fa"] = D("شرکای تجاری با موفقیت دریافت شدند.", "شریک تجاری با موفقیت ایجاد شد.", "تعاریف با موفقیت دریافت شدند.", "کد و نام الزامی است.", "کد شریک تجاری از قبل وجود دارد.", "شماره شناسایی باید ۱۱ نویسه باشد."),
            ["zh"] = D("业务伙伴获取成功。", "业务伙伴创建成功。", "业务伙伴定义获取成功。", "代码和名称为必填项。", "业务伙伴代码已存在。", "身份证号必须为11位。"),
            ["ja"] = D("取引先を取得しました。", "取引先を作成しました。", "取引先定義を取得しました。", "コードと名前は必須です。", "取引先コードは既に存在します。", "識別番号は11文字である必要があります。"),
            ["ko"] = D("비즈니스 파트너를 불러왔습니다.", "비즈니스 파트너를 생성했습니다.", "비즈니스 파트너 정의를 불러왔습니다.", "코드와 이름은 필수입니다.", "비즈니스 파트너 코드가 이미 존재합니다.", "식별 번호는 11자여야 합니다.")
        };

    private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> RuleResources =
        new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["tr"] = R("Vergi numarası zorunludur.", "Vergi dairesi zorunludur.", "E-posta adresi zorunludur.", "Telefon veya cep telefonu zorunludur.", "T.C. kimlik numarası zorunludur.", "Bu vergi numarası başka bir caride kullanılıyor.", "Bu kimlik numarası başka bir caride kullanılıyor.", "Bu e-posta adresi başka bir caride kullanılıyor."),
            ["en"] = R("Tax number is required.", "Tax office is required.", "Email address is required.", "Phone or mobile phone is required.", "National identity number is required.", "This tax number is already used by another business partner.", "This identity number is already used by another business partner.", "This email address is already used by another business partner."),
            ["de"] = R("Die Steuernummer ist erforderlich.", "Das Finanzamt ist erforderlich.", "Die E-Mail-Adresse ist erforderlich.", "Telefon oder Mobiltelefon ist erforderlich.", "Die nationale ID ist erforderlich.", "Diese Steuernummer wird bereits verwendet.", "Diese ID wird bereits verwendet.", "Diese E-Mail-Adresse wird bereits verwendet."),
            ["fr"] = R("Le numéro fiscal est obligatoire.", "Le centre fiscal est obligatoire.", "L’adresse e-mail est obligatoire.", "Le téléphone fixe ou mobile est obligatoire.", "Le numéro d’identité est obligatoire.", "Ce numéro fiscal est déjà utilisé.", "Ce numéro d’identité est déjà utilisé.", "Cette adresse e-mail est déjà utilisée."),
            ["es"] = R("El número fiscal es obligatorio.", "La oficina fiscal es obligatoria.", "El correo electrónico es obligatorio.", "El teléfono fijo o móvil es obligatorio.", "El número de identidad es obligatorio.", "Este número fiscal ya está en uso.", "Este número de identidad ya está en uso.", "Este correo electrónico ya está en uso."),
            ["it"] = R("Il codice fiscale è obbligatorio.", "L’ufficio fiscale è obbligatorio.", "L’indirizzo e-mail è obbligatorio.", "Il telefono fisso o mobile è obbligatorio.", "Il numero identificativo è obbligatorio.", "Questo codice fiscale è già utilizzato.", "Questo numero identificativo è già utilizzato.", "Questa e-mail è già utilizzata."),
            ["pt"] = R("O número fiscal é obrigatório.", "A repartição fiscal é obrigatória.", "O e-mail é obrigatório.", "O telefone fixo ou celular é obrigatório.", "O número de identificação é obrigatório.", "Este número fiscal já está em uso.", "Este número de identificação já está em uso.", "Este e-mail já está em uso."),
            ["nl"] = R("Het belastingnummer is verplicht.", "Het belastingkantoor is verplicht.", "Het e-mailadres is verplicht.", "Telefoon of mobiel nummer is verplicht.", "Het identiteitsnummer is verplicht.", "Dit belastingnummer is al in gebruik.", "Dit identiteitsnummer is al in gebruik.", "Dit e-mailadres is al in gebruik."),
            ["pl"] = R("Numer podatkowy jest wymagany.", "Urząd skarbowy jest wymagany.", "Adres e-mail jest wymagany.", "Telefon lub telefon komórkowy jest wymagany.", "Numer identyfikacyjny jest wymagany.", "Ten numer podatkowy jest już używany.", "Ten numer identyfikacyjny jest już używany.", "Ten adres e-mail jest już używany."),
            ["ru"] = R("Налоговый номер обязателен.", "Налоговая инспекция обязательна.", "Адрес электронной почты обязателен.", "Телефон или мобильный телефон обязателен.", "Идентификационный номер обязателен.", "Этот налоговый номер уже используется.", "Этот идентификационный номер уже используется.", "Этот адрес электронной почты уже используется."),
            ["ar"] = R("الرقم الضريبي مطلوب.", "مكتب الضرائب مطلوب.", "عنوان البريد الإلكتروني مطلوب.", "رقم الهاتف أو الجوال مطلوب.", "رقم الهوية مطلوب.", "الرقم الضريبي مستخدم بالفعل.", "رقم الهوية مستخدم بالفعل.", "عنوان البريد الإلكتروني مستخدم بالفعل."),
            ["fa"] = R("شماره مالیاتی الزامی است.", "اداره مالیات الزامی است.", "نشانی ایمیل الزامی است.", "تلفن یا تلفن همراه الزامی است.", "شماره شناسایی الزامی است.", "این شماره مالیاتی قبلاً استفاده شده است.", "این شماره شناسایی قبلاً استفاده شده است.", "این نشانی ایمیل قبلاً استفاده شده است."),
            ["zh"] = R("税号为必填项。", "税务机关为必填项。", "电子邮件为必填项。", "电话或手机为必填项。", "身份证号为必填项。", "该税号已被其他业务伙伴使用。", "该身份证号已被其他业务伙伴使用。", "该电子邮件已被其他业务伙伴使用。"),
            ["ja"] = R("税務番号は必須です。", "税務署は必須です。", "メールアドレスは必須です。", "電話または携帯電話は必須です。", "識別番号は必須です。", "この税務番号は既に使用されています。", "この識別番号は既に使用されています。", "このメールアドレスは既に使用されています。"),
            ["ko"] = R("세금 번호는 필수입니다.", "세무서는 필수입니다.", "이메일 주소는 필수입니다.", "전화 또는 휴대폰 번호는 필수입니다.", "식별 번호는 필수입니다.", "이 세금 번호는 이미 사용 중입니다.", "이 식별 번호는 이미 사용 중입니다.", "이 이메일 주소는 이미 사용 중입니다.")
        };

    private static IReadOnlyDictionary<string, string> D(string retrieved, string created, string definitions, string required, string duplicate, string invalidId) =>
        new Dictionary<string, string> { ["Retrieved"] = retrieved, ["Created"] = created, ["DefinitionsRetrieved"] = definitions,
            ["Required"] = required, ["DuplicateCode"] = duplicate, ["InvalidTckn"] = invalidId };

    private static IReadOnlyDictionary<string, string> R(string taxRequired, string taxOfficeRequired, string emailRequired,
        string phoneRequired, string nationalIdRequired, string duplicateTax, string duplicateNationalId, string duplicateEmail) =>
        new Dictionary<string, string> { ["TaxNumberRequired"] = taxRequired, ["TaxOfficeRequired"] = taxOfficeRequired,
            ["EmailRequired"] = emailRequired, ["PhoneRequired"] = phoneRequired, ["NationalIdRequired"] = nationalIdRequired,
            ["DuplicateTaxNumber"] = duplicateTax, ["DuplicateNationalId"] = duplicateNationalId, ["DuplicateEmail"] = duplicateEmail };

    public static string Get(string key, string? culture)
    {
        var language = culture?.Split(',')[0].Split('-')[0].ToLowerInvariant() ?? "tr";
        if (!Resources.ContainsKey(language)) language = "tr";
        if (Resources[language].TryGetValue(key, out var value)) return value;
        return RuleResources[language].TryGetValue(key, out value) ? value : key;
    }
}
