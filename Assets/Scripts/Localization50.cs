using System.Collections.Generic;

public static class Localization50
{
    public static readonly string[] Codes = new string[]
    {
        "en","fa","ar","tr","ru","de","fr","es","it","pt","nl","sv","no","da","fi","pl","cs","sk","hu","ro",
        "bg","hr","sr","uk","el","he","hi","bn","ur","id","ms","th","vi","zh","ja","ko","ta","te","mr","gu",
        "pa","kn","ml","my","km","lo","ka","am","sw","ha"
    };

    public static readonly string[] Names = new string[]
    {
        "English","فارسی","العربية","Türkçe","Русский","Deutsch","Français","Español","Italiano","Português",
        "Nederlands","Svenska","Norsk","Dansk","Suomi","Polski","Čeština","Slovenčina","Magyar","Română",
        "Български","Hrvatski","Srpski","Українська","Ελληνικά","עברית","हिन्दी","বাংলা","اردو","Bahasa Indonesia",
        "Bahasa Melayu","ไทย","Tiếng Việt","中文","日本語","한국어","தமிழ்","తెలుగు","मराठी","ગુજરાતી",
        "ਪੰਜਾਬੀ","ಕನ್ನಡ","മലയാളം","မြန်မာ","ខ្មែរ","ລາວ","ქართული","አማርኛ","Kiswahili","Hausa"
    };

    public static readonly string[] Keys = new string[]
    {
        "Play","BattleRoyale","Multiplayer","Settings","Credits","Language","Sensitivity","Quality",
        "Fire","Reload","Jump","Grenade","Medkit","ADS","Health","Ammo","Score","Alive","Winner",
        "Back","Start","Pause","Resume","Restart"
    };

    private static readonly Dictionary<string, string[]> Table = new Dictionary<string, string[]>()
    {
        {"en", new string[]{"Play","Battle Royale","Multiplayer","Settings","Credits","Language","Sensitivity","Quality","Fire","Reload","Jump","Grenade","Medkit","ADS","Health","Ammo","Score","Alive","Winner","Back","Start","Pause","Resume","Restart"}},
        {"fa", new string[]{"بازی","بتل‌رویال","مولتی‌پلیر","تنظیمات","سازندگان","زبان","حساسیت","کیفیت","شلیک","پر کردن","پرش","نارنجک","جعبه کمک‌های اولیه","نشانه‌گیری","جان","مهمات","امتیاز","زنده","برنده","برگشت","شروع","توقف","ادامه","شروع دوباره"}},
        {"ar", new string[]{"العب","باتل رويال","متعدد اللاعبين","الإعدادات","الاعتمادات","اللغة","الحساسية","الجودة","إطلاق","إعادة التحميل","قفز","قنبلة","حقيبة إسعاف","التصويب","الصحة","الذخيرة","النقاط","أحياء","الفائز","رجوع","ابدأ","إيقاف","استئناف","إعادة"}},
        {"tr", new string[]{"Oyna","Battle Royale","Çok Oyunculu","Ayarlar","Künye","Dil","Hassasiyet","Kalite","Ateş","Yeniden Doldur","Zıpla","El Bombası","İlk Yardım","Nişan","Can","Cephane","Skor","Hayatta","Kazanan","Geri","Başla","Duraklat","Devam","Yeniden Başlat"}},
        {"ru", new string[]{"Играть","Королевская битва","Мультиплеер","Настройки","Авторы","Язык","Чувствительность","Качество","Огонь","Перезарядка","Прыжок","Граната","Аптечка","Прицел","Здоровье","Боеприпасы","Счёт","Живы","Победитель","Назад","Старт","Пауза","Продолжить","Заново"}},
        {"de", new string[]{"Spielen","Battle Royale","Mehrspieler","Einstellungen","Credits","Sprache","Empfindlichkeit","Qualität","Feuer","Nachladen","Springen","Granate","Medkit","Zielen","Leben","Munition","Punkte","Lebendig","Sieger","Zurück","Start","Pause","Weiter","Neustart"}},
        {"fr", new string[]{"Jouer","Battle Royale","Multijoueur","Paramètres","Crédits","Langue","Sensibilité","Qualité","Tir","Recharger","Sauter","Grenade","Trousse de secours","Visée","Santé","Munitions","Score","En vie","Gagnant","Retour","Démarrer","Pause","Reprendre","Recommencer"}},
        {"es", new string[]{"Jugar","Battle Royale","Multijugador","Ajustes","Créditos","Idioma","Sensibilidad","Calidad","Disparar","Recargar","Saltar","Granada","Botiquín","Apuntar","Salud","Munición","Puntos","Vivos","Ganador","Atrás","Empezar","Pausa","Continuar","Reiniciar"}},
        {"it", new string[]{"Gioca","Battle Royale","Multigiocatore","Impostazioni","Crediti","Lingua","Sensibilità","Qualità","Fuoco","Ricarica","Salta","Granata","Kit medico","Mira","Salute","Munizioni","Punteggio","Vivi","Vincitore","Indietro","Inizia","Pausa","Riprendi","Riavvia"}},
        {"pt", new string[]{"Jogar","Battle Royale","Multijogador","Configurações","Créditos","Idioma","Sensibilidade","Qualidade","Atirar","Recarregar","Pular","Granada","Kit médico","Mirar","Vida","Munição","Pontos","Vivos","Vencedor","Voltar","Começar","Pausa","Continuar","Reiniciar"}},
        {"nl", new string[]{"Spelen","Battle Royale","Multiplayer","Instellingen","Credits","Taal","Gevoeligheid","Kwaliteit","Vuur","Herladen","Springen","Granaat","EHBO-kit","Richten","Gezondheid","Munitie","Score","Levend","Winnaar","Terug","Start","Pauze","Hervat","Opnieuw"}},
        {"sv", new string[]{"Spela","Battle Royale","Flera spelare","Inställningar","Credits","Språk","Känslighet","Kvalitet","Eld","Ladda om","Hoppa","Granat","Första hjälpen-kit","Sikta","Hälsa","Ammunition","Poäng","Vid liv","Vinnare","Tillbaka","Starta","Paus","Fortsätt","Starta om"}},
        {"no", new string[]{"Spill","Battle Royale","Flerspiller","Innstillinger","Credits","Språk","Følsomhet","Kvalitet","Ild","Lad om","Hopp","Granat","Førstehjelpsskrin","Sikte","Helse","Ammunisjon","Poeng","I live","Vinner","Tilbake","Start","Pause","Fortsett","Start på nytt"}},
        {"da", new string[]{"Spil","Battle Royale","Multiplayer","Indstillinger","Credits","Sprog","Følsomhed","Kvalitet","Ild","Genlad","Hop","Granat","Førstehjælp","Sigte","Sundhed","Ammunition","Point","I live","Vinder","Tilbage","Start","Pause","Fortsæt","Genstart"}},
        {"fi", new string[]{"Pelaa","Battle Royale","Moninpeli","Asetukset","Tekijät","Kieli","Herkkyys","Laatu","Tuli","Lataa uudelleen","Hyppää","Kranaatti","Ensiapupakkaus","Tähtää","Terveys","Ammukset","Pisteet","Elossa","Voittaja","Takaisin","Aloita","Tauko","Jatka","Aloita uudelleen"}},
        {"pl", new string[]{"Graj","Battle Royale","Multiplayer","Ustawienia","Twórcy","Język","Czułość","Jakość","Ogień","Przeładuj","Skacz","Granat","Apteczka","Celowanie","Zdrowie","Amunicja","Wynik","Żywi","Zwycięzca","Wstecz","Start","Pauza","Wznów","Restart"}},
        {"cs", new string[]{"Hrát","Battle Royale","Multiplayer","Nastavení","Titulky","Jazyk","Citlivost","Kvalita","Palba","Přebít","Skočit","Granát","Lékárnička","Míření","Zdraví","Munice","Skóre","Živí","Vítěz","Zpět","Start","Pauza","Pokračovat","Restart"}},
        {"sk", new string[]{"Hrať","Battle Royale","Multiplayer","Nastavenia","Titulky","Jazyk","Citlivosť","Kvalita","Paľba","Prebiť","Skočiť","Granát","Lekárnička","Mierenie","Zdravie","Munícia","Skóre","Živí","Víťaz","Späť","Štart","Pauza","Pokračovať","Reštart"}},
        {"hu", new string[]{"Játék","Battle Royale","Többjátékos","Beállítások","Készítők","Nyelv","Érzékenység","Minőség","Tűz","Újratöltés","Ugrás","Gránát","Elsősegély","Célzás","Élet","Lőszer","Pontszám","Élők","Győztes","Vissza","Indítás","Szünet","Folytatás","Újraindítás"}},
        {"ro", new string[]{"Joacă","Battle Royale","Multiplayer","Setări","Credite","Limbă","Sensibilitate","Calitate","Foc","Reîncarcă","Sari","Grenadă","Trusă medicală","Țintire","Sănătate","Muniție","Scor","În viață","Câștigător","Înapoi","Start","Pauză","Continuă","Reîncepe"}},
        {"bg", new string[]{"Играй","Battle Royale","Мултиплеър","Настройки","Автори","Език","Чувствителност","Качество","Огън","Презареди","Скок","Граната","Аптечка","Прицел","Здраве","Муниции","Резултат","Живи","Победител","Назад","Старт","Пауза","Продължи","Рестарт"}},
        {"hr", new string[]{"Igraj","Battle Royale","Za više igrača","Postavke","Zasluge","Jezik","Osjetljivost","Kvaliteta","Vatra","Ponovno napuni","Skoči","Bomba","Prva pomoć","Nišanjenje","Zdravlje","Streljivo","Bodovi","Živi","Pobjednik","Natrag","Počni","Pauza","Nastavi","Ponovno pokreni"}},
        {"sr", new string[]{"Igraj","Battle Royale","Multiplayer","Podešavanja","Zasluge","Jezik","Osetljivost","Kvalitet","Vatra","Dopuni","Skoči","Bomba","Prva pomoć","Nišanjenje","Zdravlje","Municija","Rezultat","Živi","Pobednik","Nazad","Počni","Pauza","Nastavi","Ponovo pokreni"}},
        {"uk", new string[]{"Грати","Королівська битва","Мультиплеєр","Налаштування","Автори","Мова","Чутливість","Якість","Вогонь","Перезаряди","Стрибни","Граната","Аптечка","Приціл","Здоров'я","Боєприпаси","Рахунок","Живі","Переможець","Назад","Старт","Пауза","Продовжити","Заново"}},
        {"el", new string[]{"Παίξε","Battle Royale","Πολλαπλών παικτών","Ρυθμίσεις","Συντελεστές","Γλώσσα","Ευαισθησία","Ποιότητα","Πυρ","Επαναφόρτιση","Άλμα","Χειροβομβίδα","Φαρμακείο","Στόχευση","Υγεία","Πυρομαχικά","Σκορ","Ζωντανοί","Νικητής","Πίσω","Έναρξη","Παύση","Συνέχεια","Επανεκκίνηση"}},
        {"he", new string[]{"שחק","באטל רויאל","מרובה משתתפים","הגדרות","קרדיטים","שפה","רגישות","איכות","אש","טען מחדש","קפוץ","רימון","ערכת עזרה ראשונה","כוון","בריאות","תחמושת","ניקוד","חיים","מנצח","חזרה","התחל","השהה","המשך","הפעל מחדש"}},
        {"hi", new string[]{"खेलें","बैटल रॉयल","मल्टीप्लेयर","सेटिंग्स","क्रेडिट","भाषा","संवेदनशीलता","गुणवत्ता","आग","पुनः लोड","कूदें","ग्रेनेड","मेडकिट","निशाना","स्वास्थ्य","गोला-बारूद","स्कोर","जीवित","विजेता","वापस","शुरू","विराम","जारी रखें","पुनः आरंभ"}},
        {"bn", new string[]{"খেলুন","ব্যাটল রয়ালে","মাল্টিপ্লেয়ার","সেটিংস","ক্রেডিট","ভাষা","সংবেদনশীলতা","গুণমান","আগুন","পুনরায় লোড","লাফ দিন","গ্রেনেড","মেডকিট","লক্ষ্য","স্বাস্থ্য","গুলি","স্কোর","জীবিত","বিজয়ী","পিছনে","শুরু","বিরতি","চালিয়ে যান","পুনরায় শুরু"}},
        {"ur", new string[]{"کھیلو","بیٹل رائل","ملٹی پلیئر","ترتیبات","کریڈٹس","زبان","حساسیت","معیار","آگ","دوبارہ لوڈ","چھلانگ","دستی بم","طبی کٹ","نشانہ","صحت","گولہ بارود","اسکور","زندہ","فاتح","پیچھے","شروع","وقفہ","جاری رکھیں","دوبارہ شروع"}},
        {"id", new string[]{"Main","Battle Royale","Multipemain","Pengaturan","Kredit","Bahasa","Sensitivitas","Kualitas","Tembak","Isi Ulang","Lompat","Granat","Kotak P3K","Bidik","Kesehatan","Amunisi","Skor","Hidup","Pemenang","Kembali","Mulai","Jeda","Lanjut","Ulangi"}},
        {"ms", new string[]{"Main","Battle Royale","Multipemain","Tetapan","Kredit","Bahasa","Kepekaan","Kualiti","Tembak","Muat Semula","Lompat","Grenad","Kit Perubatan","Aim","Kesihatan","Peluru","Skor","Hidup","Pemenang","Kembali","Mula","Jeda","Teruskan","Mula Semula"}},
        {"th", new string[]{"เล่น","แบทเทิลรอยัล","หลายผู้เล่น","การตั้งค่า","เครดิต","ภาษา","ความไว","คุณภาพ","ยิง","โหลดใหม่","กระโดด","ระเบิด","ชุดปฐมพยาบาล","เล็ง","สุขภาพ","กระสุน","คะแนน","ยังมีชีวิต","ผู้ชนะ","กลับ","เริ่ม","หยุด","ดำเนินการต่อ","เริ่มใหม่"}},
        {"vi", new string[]{"Chơi","Battle Royale","Nhiều người chơi","Cài đặt","Tín dụng","Ngôn ngữ","Độ nhạy","Chất lượng","Bắn","Nạp lại","Nhảy","Lựu đạn","Túi y tế","Ngắm","Máu","Đạn","Điểm","Còn sống","Người thắng","Quay lại","Bắt đầu","Tạm dừng","Tiếp tục","Chơi lại"}},
        {"zh", new string[]{"开始游戏","大逃杀","多人","设置","制作人员","语言","灵敏度","画质","开火","装弹","跳跃","手雷","医疗包","瞄准","生命","弹药","分数","存活","胜利者","返回","开始","暂停","继续","重新开始"}},
        {"ja", new string[]{"プレイ","バトルロイヤル","マルチプレイヤー","設定","クレジット","言語","感度","画質","射撃","リロード","ジャンプ","グレネード","メドキット","エイム","体力","弾薬","スコア","生存者","勝者","戻る","スタート","ポーズ","再開","リスタート"}},
        {"ko", new string[]{"플레이","배틀 로얄","멀티플레이어","설정","크레딧","언어","감도","품질","사격","재장전","점프","수류탄","메드킷","조준","체력","탄약","점수","생존","승자","뒤로","시작","일시정지","계속","다시 시작"}},
        {"ta", new string[]{"விளையாடு","பேட்டில் ராயல்","மல்டிபிளேயர்","அமைப்புகள்","வரவுகள்","மொழி","உணர்திறன்","தரம்","சுடு","மீண்டும் ஏற்று","குதி","குண்டு","முதலுதவி பெட்டி","குறிவை","உடல்நலம்","வெடிமருந்து","மதிப்பெண்","உயிருடன்","வெற்றியாளர்","பின்","தொடங்கு","இடைநிறுத்து","தொடர்","மீண்டும் தொடங்கு"}},
        {"te", new string[]{"ఆడు","బ్యాటిల్ రాయల్","మల్టీప్లేయర్","సెట్టింగులు","క్రెడిట్స్","భాష","సెన్సిటివిటీ","నాణ్యత","కాల్చు","మళ్లీ లోడ్","దోక","గ్రెనేడ్","మెడ్‌కిట్","గురి","ఆరోగ్యం","మందుగుండ్లు","స్కోర్","సజీవంగా","విజేత","వెనుకకు","ప్రారంభించు","ఆగు","కొనసాగించు","మళ్లీ ప్రారంభించు"}},
        {"mr", new string[]{"खेळा","बॅटल रॉयल","मल्टीप्लेयर","सेटिंग्ज","क्रेडिट","भाषा","संवेदनशीलता","गुणवत्ता","आग","पुन्हा लोड","उडी","ग्रेनेड","मेडकिट","नेम","आरोग्य","दारूगोळा","स्कोअर","जिवंत","विजेता","मागे","सुरू","विराम","पुढे","पुन्हा सुरू"}},
        {"gu", new string[]{"રમો","બેટલ રોયલ","મલ્ટિપ્લેયર","સેટિંગ્સ","ક્રેડિટ","ભાષા","સંવેદનશીલતા","ગુણવત્તા","આગ","ફરી લોડ","કૂદકો","ગ્રેનેડ","મેડકિટ","નિશાન","આરોગ્ય","દારૂગોળો","સ્કોર","જીવંત","વિજેતા","પાછળ","શરૂ","વિરામ","ચાલુ રાખો","ફરી શરૂ"}},
        {"pa", new string[]{"ਖੇਡੋ","ਬੈਟਲ ਰਾਇਲ","ਮਲਟੀਪਲੇਅਰ","ਸੈਟਿੰਗਾਂ","ਕ੍ਰੈਡਿਟ","ਭਾਸ਼ਾ","ਸੰਵੇਦਨਸ਼ੀਲਤਾ","ਗੁਣਵੱਤਾ","ਅੱਗ","ਮੁੜ ਲੋਡ","ਛਾਲ","ਗਰਨੇਡ","ਮੈਡਕਿਟ","ਨਿਸ਼ਾਨਾ","ਸਿਹਤ","ਗੋਲੀ-ਸਿੱਕਾ","ਸਕੋਰ","ਜਿਉਂਦੇ","ਜੇਤੂ","ਪਿੱਛੇ","ਸ਼ੁਰੂ","ਰੁਕੋ","ਜਾਰੀ ਰੱਖੋ","ਮੁੜ ਸ਼ੁਰੂ"}},
        {"kn", new string[]{"ಆಡು","ಬ್ಯಾಟಲ್ ರಾಯಲ್","ಮಲ್ಟಿಪ್ಲೇಯರ್","ಸೆಟ್ಟಿಂಗ್‌ಗಳು","ಕ್ರೆಡಿಟ್‌ಗಳು","ಭಾಷೆ","ಸೂಕ್ಷ್ಮತೆ","ಗುಣಮಟ್ಟ","ಗುಂಡು","ಮತ್ತೆ ಲೋಡ್","ಜಂಪ್","ಗ್ರೆನೇಡ್","ಮೆಡ್‌ಕಿಟ್","ಗುರಿ","ಆರೋಗ್ಯ","ಮದ್ದು","ಸ್ಕೋರ್","ಜೀವಂತ","ವಿಜೇತ","ಹಿಂದೆ","ಪ್ರಾರಂಭ","ವಿರಾಮ","ಮುಂದುವರಿಸಿ","ಮತ್ತೆ ಪ್ರಾರಂಭ"}},
        {"ml", new string[]{"കളിക്കൂ","ബാറ്റിൽ റോയൽ","മൾട്ടിപ്ലെയർ","ക്രമീകരണങ്ങൾ","ക്രെഡിറ്റുകൾ","ഭാഷ","സെൻസിറ്റിവിറ്റി","ഗുണനിലവാരം","വെടിവെക്കുക","വീണ്ടും ലോഡ്","ചാടുക","ഗ്രനേഡ്","മെഡ്കിറ്റ്","ലക്ഷ്യം","ആരോഗ്യം","വെടിമരുന്ന്","സ്കോർ","ജീവനോടെ","വിജയി","തിരികെ","ആരംഭിക്കുക","നിർത്തുക","തുടരുക","വീണ്ടും ആരംഭിക്കുക"}},
        {"my", new string[]{"ကစားပါ","ဘတ်တယ်ရိုယယ်","မလ်တီပလေယာ","ဆက်တင်များ","ခရက်ဒစ်","ဘာသာစကား","အာရုံခံနိုင်မှု","အရည်အသွေး","ပစ်ခတ်","ပြန်ဖြည့်","ခုန်ကျော်","ဗုံးသီး","ဆေးသေတ္တာများ","ချိန်ရွယ်","ကျန်းမာရေး","ကျည်ဆန်","အမှတ်","ရှင်သန်","အနိုင်ရ","နောက်သို့","စတင်","ရပ်နား","ဆက်လုပ်ပါ","ပြန်စတင်"}},
        {"km", new string[]{"លេង","បែតថលរ៉ូយ៉ាល់","ច្រើនអ្នកលេង","ការកំណត់","ឥណទាន","ភាសា","ភាពរសើប","គុណភាព","បាញ់","ផ្ទុកឡើងវិញ","លោត","គ្រាប់បែក","កញ្ចប់ពេទ្យ","កំណត់គោល","សុខភាព","គ្រាប់","ពិន្ទុ","នៅរស់","អ្នកឈ្នះ","ត្រឡប់","ចាប់ផ្តើម","ផ្អាក","បន្ត","ចាប់ផ្តើមឡើងវិញ"}},
        {"lo", new string[]{"ຫຼິ້ນ","ແບັດເທິນຣອຍແອວ","ຫຼາຍຜູ້ຫຼິ້ນ","ການຕັ້ງຄ່າ","ເຄຣດິດ","ພາສາ","ຄວາມໄວ","ຄຸນນະພາບ","ຍິງ","ໂຫຼດໃໝ່","ໂດດ","ລະເບີດ","ຊຸດແພດ","ເລັ່ງ","ສຸຂະພາບ","ກະສຸນ","ຄະແນນ","ຍັງມີຊີວິດ","ຜູ້ຊະນະ","ກັບຄືນ","ເລີ່ມຕົ້ນ","ຢຸດ","ສືບຕໍ່","ເລີ່ມໃໝ່"}},
        {"ka", new string[]{"ითამაშე","ბატლ როიალი","მულტიპლეიერი","პარამეტრები","კრედიტები","ენა","მგრძნობელობა","ხარისხი","სროლა","გადატენვა","ხტომა","ყუმბარა","სამედიცინო ყუთი","მიზანი","ჯანმრთელობა","ტყვია","ქულა","ცოცხლები","გამარჯვებული","უკან","დაწყება","პაუზა","გაგრძელება","თავიდან"}},
        {"am", new string[]{"ጫወት","ባትል ሮያል","ባለብዙ ተጫዋች","ቅንብሮች","ክሬዲቶች","ቋንቋ","ስሜታዊነት","ጥራት","ተኩስ","እንደገና ጭነቱ","ዝለል","ቦምብ","የህክምና ሳጥን","ዒላማ","ጤና","ጥይት","ነጥብ","ህያው","አሸናፊ","ተመለስ","ጀምር","አቁም","ቀጥል","እንደገና ጀምር"}},
        {"sw", new string[]{"Cheza","Battle Royale","Wachezaji wengi","Mipangilio","Sifa","Lugha","Unyeti","Ubora","Moto","Pakia upya","Ruka","Bomu","Kifaa cha matibabu","Lenga","Afya","Risasi","Alama","Wanaishi","Mshindi","Rudi","Anza","Sitisha","Endelea","Anza upya"}},
        {"ha", new string[]{"Yi wasa","Battle Royale","'Yan wasa da yawa","Saituna","Krediti","Harshe","Tashin hankali","Inganci","Wuta","Sake sake lodawa","Tsallake","Bam","Akatin magani","Nuni","Lafiya","Albaridu","Maki","Rayayyu","Mai nasara","Koma","Fara","Tsaya","Ci gaba","Sake farawa"}}
    };

    public static int IndexOf(string code)
    {
        for (int i = 0; i < Codes.Length; i++)
            if (Codes[i] == code) return i;
        return 0;
    }

    public static string Get(string code, string key)
    {
        int ki = -1;
        for (int i = 0; i < Keys.Length; i++)
            if (Keys[i] == key) { ki = i; break; }
        if (ki < 0) return key;
        string[] arr;
        if (Table.TryGetValue(code, out arr) && ki < arr.Length) return arr[ki];
        if (Table.TryGetValue("en", out arr) && ki < arr.Length) return arr[ki];
        return key;
    }
}
