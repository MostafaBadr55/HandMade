import { Lang } from './i18n.types';

export type TranslationKey =
  // Common
  | 'app.name'
  | 'common.loading'
  | 'common.retry'
  | 'common.tryAgain'
  | 'common.cancel'
  | 'common.save'
  | 'common.edit'
  | 'common.delete'
  | 'common.add'
  | 'common.create'
  | 'common.update'
  | 'common.search'
  | 'common.apply'
  | 'common.back'
  | 'common.skip'
  | 'common.yes'
  | 'common.no'
  | 'common.all'
  | 'common.active'
  | 'common.inactive'
  | 'common.pending'
  | 'common.approved'
  | 'common.rejected'
  | 'common.name'
  | 'common.description'
  | 'common.price'
  | 'common.status'
  | 'common.actions'
  | 'common.image'
  | 'common.noResults'
  // Navigation
  | 'nav.home'
  | 'nav.categories'
  | 'nav.login'
  | 'nav.logout'
  | 'nav.register'
  | 'nav.language'
  | 'nav.profile'
  | 'nav.myShop'
  | 'nav.adminDashboard'
  // Footer
  | 'footer.tagline'
  | 'footer.usefulLinks'
  | 'footer.customerServices'
  | 'footer.orderTracking'
  | 'footer.wishlist'
  | 'footer.payment'
  | 'footer.aboutUs'
  | 'footer.contactUs'
  | 'footer.ourBlog'
  | 'footer.faqs'
  | 'footer.newsletter'
  | 'footer.newsletterText'
  | 'footer.subscribe'
  | 'footer.emailUs'
  | 'footer.callUs'
  | 'footer.ourLocation'
  | 'footer.copyright'
  // Landing page
  | 'landing.hero.title'
  | 'landing.hero.subtitle'
  | 'landing.hero.cta.primary'
  | 'landing.hero.cta.secondary'
  | 'landing.searchPlaceholder'
  | 'landing.contributeTitle'
  | 'landing.festiveTitle'
  | 'landing.images'
  | 'landing.noProducts'
  | 'landing.categories.title'
  | 'landing.categories.subtitle'
  | 'landing.sidebar.categories'
  | 'landing.sidebar.subscribe.title'
  | 'landing.sidebar.subscribe.text'
  | 'landing.sidebar.subscribe.placeholder'
  | 'landing.sidebar.subscribe.button'
  | 'landing.how.title'
  | 'landing.how.step1.title'
  | 'landing.how.step1.text'
  | 'landing.how.step2.title'
  | 'landing.how.step2.text'
  | 'landing.how.step3.title'
  | 'landing.how.step3.text'
  | 'landing.value.title'
  | 'landing.value.bullet1'
  | 'landing.value.bullet2'
  | 'landing.value.bullet3'
  | 'landing.cta.title'
  | 'landing.cta.text'
  | 'landing.cta.button'
  // Subcategories
  | 'subcategories.title'
  | 'subcategories.back'
  | 'subcategories.empty'
  // Auth
  | 'auth.login'
  | 'auth.register'
  | 'auth.logout'
  | 'auth.createAccount'
  | 'auth.joinCommunity'
  | 'auth.welcomeBack'
  | 'auth.loggingIn'
  | 'auth.registering'
  | 'auth.username'
  | 'auth.email'
  | 'auth.phone'
  | 'auth.password'
  | 'auth.confirmPassword'
  | 'auth.usernameRequired'
  | 'auth.usernameMinLength'
  | 'auth.emailRequired'
  | 'auth.emailInvalid'
  | 'auth.phoneRequired'
  | 'auth.phoneInvalid'
  | 'auth.passwordRequired'
  | 'auth.passwordMinLength'
  | 'auth.passwordPattern'
  | 'auth.passwordHint'
  | 'auth.confirmPasswordRequired'
  | 'auth.passwordMismatch'
  | 'auth.forgotPassword'
  | 'auth.alreadyHaveAccount'
  | 'auth.dontHaveAccount'
  | 'auth.join'
  // Select role
  | 'role.chooseRole'
  | 'role.selectBuyerOrSeller'
  | 'role.buyer'
  | 'role.seller'
  | 'role.buyerDesc'
  | 'role.sellerDesc'
  | 'role.skipForNow'
  // Shop
  | 'shop.createShop'
  | 'shop.createYourShop'
  | 'shop.noShopYet'
  | 'shop.startNow'
  | 'shop.createNewShop'
  | 'shop.pendingApproval'
  | 'shop.pendingMessage'
  | 'shop.shopDetails'
  | 'shop.shopName'
  | 'shop.shopImage'
  | 'shop.rejected'
  | 'shop.rejectionReason'
  | 'shop.noReasonProvided'
  | 'shop.rejectionHelp'
  | 'shop.editShop'
  | 'shop.saveChanges'
  | 'shop.creating'
  | 'shop.manageShop'
  | 'shop.manageCategories'
  | 'shop.manageSubCategories'
  // Products
  | 'product.myProducts'
  | 'product.addProduct'
  | 'product.addNewProduct'
  | 'product.addFirstProduct'
  | 'product.noProducts'
  | 'product.totalProducts'
  | 'product.activeProducts'
  | 'product.inactiveProducts'
  | 'product.pendingApproval'
  | 'product.productTitle'
  | 'product.enterTitle'
  | 'product.category'
  | 'product.selectCategory'
  | 'product.subcategory'
  | 'product.selectSubcategory'
  | 'product.productImage'
  | 'product.adding'
  // Admin
  | 'admin.dashboard'
  | 'admin.backToSite'
  | 'admin.pendingShops'
  | 'admin.allShops'
  | 'admin.reviewApplications'
  | 'admin.allCaughtUp'
  | 'admin.noPendingApps'
  | 'admin.approve'
  | 'admin.reject'
  | 'admin.rejectionReason'
  | 'admin.sendAndReject'
  | 'admin.rejectPlaceholder'
  | 'admin.owner'
  | 'admin.noDescription'
  | 'admin.loadingRequests'
  | 'admin.shops'
  | 'admin.products'
  | 'admin.users'
  | 'admin.categories'
  | 'admin.subcategories'
  | 'admin.id'
  | 'admin.searchByName'
  | 'admin.allStatus'
  | 'admin.allApprovalStatus'
  | 'admin.noPendingShops'
  | 'admin.noShopsFound'
  | 'admin.noProductsFound'
  | 'admin.noCategoriesFound'
  | 'admin.noSubcategoriesFound'
  | 'admin.title'
  | 'admin.approval'
  | 'admin.assignRole'
  | 'admin.enterUsername'
  | 'admin.selectRole'
  | 'admin.addCategory'
  | 'admin.editCategory'
  | 'admin.categoryName'
  | 'admin.imageUrl'
  | 'admin.addSubcategory'
  | 'admin.editSubcategory'
  | 'admin.subcategoryName'
  | 'admin.categoryId'
  | 'admin.rejectShop'
  | 'admin.rejectProduct'
  | 'admin.rejectionMessage'
  | 'admin.enterRejectionReason'
  // Cart
  | 'cart.title'
  | 'cart.items'
  | 'cart.empty'
  | 'cart.emptyMessage'
  | 'cart.continueShopping'
  | 'cart.subtotal'
  | 'cart.total'
  | 'cart.checkout'
  | 'cart.clearCart'
  | 'cart.remove'
  | 'cart.orderSummary'
  | 'cart.addToCart'
  | 'cart.addedToCart';

export const DICTIONARY: Record<Lang, Record<TranslationKey, string>> = {
  ar: {
    // Common
    'app.name': 'هاند ميد',
    'common.loading': 'جاري التحميل...',
    'common.retry': 'إعادة المحاولة',
    'common.tryAgain': 'حاول مرة أخرى',
    'common.cancel': 'إلغاء',
    'common.save': 'حفظ',
    'common.edit': 'تعديل',
    'common.delete': 'حذف',
    'common.add': 'إضافة',
    'common.create': 'إنشاء',
    'common.update': 'تحديث',
    'common.search': 'بحث',
    'common.apply': 'تطبيق',
    'common.back': 'رجوع',
    'common.skip': 'تخطي',
    'common.yes': 'نعم',
    'common.no': 'لا',
    'common.all': 'الكل',
    'common.active': 'نشط',
    'common.inactive': 'غير نشط',
    'common.pending': 'قيد المراجعة',
    'common.approved': 'تمت الموافقة',
    'common.rejected': 'مرفوض',
    'common.name': 'الاسم',
    'common.description': 'الوصف',
    'common.price': 'السعر',
    'common.status': 'الحالة',
    'common.actions': 'الإجراءات',
    'common.image': 'الصورة',
    'common.noResults': 'لا توجد نتائج',
    // Navigation
    'nav.home': 'الرئيسية',
    'nav.categories': 'التصنيفات',
    'nav.login': 'تسجيل الدخول',
    'nav.logout': 'تسجيل الخروج',
    'nav.register': 'إنشاء حساب',
    'nav.language': 'اللغة',
    'nav.profile': 'الملف الشخصي',
    'nav.myShop': 'متجري',
    'nav.adminDashboard': 'لوحة التحكم',
    // Footer
    'footer.tagline': 'منصة لبيع أعمال طلاب فنون جميلة بأسعار رمزية.',
    'footer.usefulLinks': 'روابط مفيدة',
    'footer.customerServices': 'خدمة العملاء',
    'footer.orderTracking': 'تتبع الطلبات',
    'footer.wishlist': 'قائمة الرغبات',
    'footer.payment': 'الدفع',
    'footer.aboutUs': 'من نحن',
    'footer.contactUs': 'تواصل معنا',
    'footer.ourBlog': 'مدونتنا',
    'footer.faqs': 'الأسئلة الشائعة',
    'footer.newsletter': 'اشترك في نشرتنا الإخبارية',
    'footer.newsletterText': 'احصل على آخر التحديثات والعروض',
    'footer.subscribe': 'اشترك',
    'footer.emailUs': 'راسلنا',
    'footer.callUs': 'اتصل بنا',
    'footer.ourLocation': 'موقعنا',
    'footer.copyright': 'جميع الحقوق محفوظة ©',
    // Landing
    'landing.hero.title': 'اعرض أعمالك الفنية… وسيصل بها المشتري المناسب',
    'landing.hero.subtitle': 'منصة تساعد طلاب فنون جميلة على بيع اللوحات والمجسمات والمشغولات بدل تخزينها في البيت — بسعر رمزي أقل من الجاليري.',
    'landing.hero.cta.primary': 'استكشف التصنيفات',
    'landing.hero.cta.secondary': 'عن المنصة',
    'landing.categories.title': 'التصنيفات',
    'landing.categories.subtitle': 'اختار القسم المناسب وشوف الأنواع الفرعية.',
    'landing.sidebar.categories': 'تصنيفات',
    'landing.sidebar.subscribe.title': 'اشترك للتحديثات',
    'landing.sidebar.subscribe.text': 'استقبل إشعارات بأعمال طلاب جديدة داخل المنصة.',
    'landing.sidebar.subscribe.placeholder': 'اكتب بريدك الإلكتروني',
    'landing.sidebar.subscribe.button': 'اشترك',
    'landing.searchPlaceholder': 'ابحث عن الصور والرسومات',
    'landing.contributeTitle': 'شارك أول صورة لك',
    'landing.festiveTitle': 'إطارات احتفالية ومساحة للنسخ',
    'landing.images': 'صورة',
    'landing.noProducts': 'لا توجد منتجات متاحة.',
    'landing.how.title': 'إزاي تشتغل المنصة؟',
    'landing.how.step1.title': 'ارفع عملك',
    'landing.how.step1.text': 'صوّر العمل واكتب تفاصيله (المقاس، الخامة، السعر).',
    'landing.how.step2.title': 'يتعرض داخل التصنيف',
    'landing.how.step2.text': 'المشتري يقدر يوصل للعمل بسهولة من التصنيفات والأنواع الفرعية.',
    'landing.how.step3.title': 'بيع بسعر عادل',
    'landing.how.step3.text': 'تسعير رمزي مناسب للطلاب وأقل من أسعار الجاليري.',
    'landing.value.title': 'ليه أنليمتد؟',
    'landing.value.bullet1': 'تساعد الطلاب يقللوا تخزين الأعمال في البيت.',
    'landing.value.bullet2': 'تسعير رمزي يناسب الطلبة والمشترين.',
    'landing.value.bullet3': 'تصنيفات واضحة للوصول السريع للأعمال.',
    'landing.cta.title': 'ابدأ من التصنيفات',
    'landing.cta.text': 'اختار التصنيف وشوف الساب كاتيجوري الخاصة به.',
    'landing.cta.button': 'عرض التصنيفات',
    // Subcategories
    'subcategories.title': 'الأنواع الفرعية',
    'subcategories.back': 'رجوع',
    'subcategories.empty': 'لا توجد أنواع فرعية متاحة حالياً لهذا التصنيف.',
    // Auth
    'auth.login': 'تسجيل الدخول',
    'auth.register': 'إنشاء حساب',
    'auth.logout': 'تسجيل الخروج',
    'auth.createAccount': 'إنشاء حساب - أنليمتد',
    'auth.joinCommunity': 'انضم لمجتمع أنليمتد اليوم',
    'auth.welcomeBack': 'مرحباً بعودتك.',
    'auth.loggingIn': 'جاري تسجيل الدخول...',
    'auth.registering': 'جاري التسجيل...',
    'auth.username': 'اسم المستخدم',
    'auth.email': 'البريد الإلكتروني',
    'auth.phone': 'رقم الهاتف',
    'auth.password': 'كلمة المرور',
    'auth.confirmPassword': 'تأكيد كلمة المرور',
    'auth.usernameRequired': 'اسم المستخدم مطلوب.',
    'auth.usernameMinLength': 'يجب أن يتكون اسم المستخدم من 3 أحرف على الأقل.',
    'auth.emailRequired': 'البريد الإلكتروني مطلوب.',
    'auth.emailInvalid': 'يرجى إدخال بريد إلكتروني صالح.',
    'auth.phoneRequired': 'رقم الهاتف مطلوب.',
    'auth.phoneInvalid': 'يرجى إدخال رقم هاتف صالح (أرقام فقط).',
    'auth.passwordRequired': 'كلمة المرور مطلوبة.',
    'auth.passwordMinLength': 'يجب أن تكون كلمة المرور 8 أحرف على الأقل.',
    'auth.passwordPattern': 'يجب أن تحتوي كلمة المرور على حرف كبير، حرف صغير، رقم ورمز.',
    'auth.passwordHint': 'نصيحة: 8+ أحرف، حرف كبير، حرف صغير، رقم ورمز.',
    'auth.confirmPasswordRequired': 'تأكيد كلمة المرور مطلوب.',
    'auth.passwordMismatch': 'كلمة المرور غير متطابقة.',
    'auth.forgotPassword': 'نسيت كلمة المرور؟',
    'auth.alreadyHaveAccount': 'لديك حساب بالفعل؟',
    'auth.dontHaveAccount': 'ليس لديك حساب؟',
    'auth.join': 'انضم',
    // Select role
    'role.chooseRole': 'اختر دورك',
    'role.selectBuyerOrSeller': 'حدد ما إذا كنت مشتري أو بائع',
    'role.buyer': 'مشتري',
    'role.seller': 'بائع',
    'role.buyerDesc': 'تصفح واشتري الأعمال الفنية',
    'role.sellerDesc': 'بيع أعمالك وإنشاء متجر',
    'role.skipForNow': 'تخطي الآن',
    // Shop
    'shop.createShop': 'إنشاء متجر',
    'shop.createYourShop': 'أنشئ متجرك الخاص',
    'shop.noShopYet': 'لم تنشئ متجرًا بعد. ابدأ الآن وشارك إبداعاتك مع العالم!',
    'shop.startNow': 'ابدأ الآن',
    'shop.createNewShop': 'إنشاء متجر جديد',
    'shop.pendingApproval': 'متجرك قيد المراجعة',
    'shop.pendingMessage': 'شكراً لإنشاء متجرك! يتم مراجعته حالياً من قبل الإدارة. ستتلقى إشعاراً عند الموافقة.',
    'shop.shopDetails': 'معلومات المتجر',
    'shop.shopName': 'اسم المتجر',
    'shop.shopImage': 'صورة المتجر',
    'shop.rejected': 'تم رفض متجرك',
    'shop.rejectionReason': 'سبب الرفض:',
    'shop.noReasonProvided': 'لم يتم توفير سبب.',
    'shop.rejectionHelp': 'يمكنك تعديل بيانات المتجر وإعادة الإرسال للمراجعة.',
    'shop.editShop': 'تعديل المتجر',
    'shop.saveChanges': 'حفظ التغييرات',
    'shop.creating': 'جاري الإنشاء...',
    'shop.manageShop': 'إدارة متجرك',
    'shop.manageCategories': 'إدارة التصنيفات',
    'shop.manageSubCategories': 'إدارة التصنيفات الفرعية',
    // Products
    'product.myProducts': 'منتجاتي',
    'product.addProduct': 'إضافة منتج',
    'product.addNewProduct': 'إضافة منتج جديد',
    'product.addFirstProduct': 'إضافة أول منتج',
    'product.noProducts': 'لا توجد منتجات في هذا القسم',
    'product.totalProducts': 'إجمالي المنتجات',
    'product.activeProducts': 'منتجات نشطة',
    'product.inactiveProducts': 'منتجات غير نشطة',
    'product.pendingApproval': 'قيد المراجعة',
    'product.productTitle': 'عنوان المنتج',
    'product.enterTitle': 'أدخل العنوان',
    'product.category': 'الفئة',
    'product.selectCategory': 'اختر الفئة',
    'product.subcategory': 'الفئة الفرعية',
    'product.selectSubcategory': 'اختر الفئة الفرعية',
    'product.productImage': 'صورة المنتج',
    'product.adding': 'جاري الإضافة...',
    // Admin
    'admin.dashboard': 'لوحة تحكم الأدمن',
    'admin.backToSite': 'العودة للموقع',
    'admin.pendingShops': 'المتاجر المعلقة',
    'admin.allShops': 'جميع المتاجر',
    'admin.reviewApplications': 'مراجعة وإدارة طلبات المتاجر',
    'admin.allCaughtUp': 'تم الانتهاء!',
    'admin.noPendingApps': 'لا توجد طلبات متاجر معلقة حالياً.',
    'admin.approve': 'موافقة',
    'admin.reject': 'رفض',
    'admin.rejectionReason': 'سبب الرفض',
    'admin.sendAndReject': 'إرسال ورفض',
    'admin.rejectPlaceholder': 'اكتب سبب الرفض سيصل للبائع',
    'admin.owner': 'المالك',
    'admin.noDescription': 'لا يوجد وصف.',
    'admin.loadingRequests': 'جاري تحميل الطلبات...',
    'admin.shops': 'المتاجر',
    'admin.products': 'المنتجات',
    'admin.users': 'المستخدمين',
    'admin.categories': 'التصنيفات',
    'admin.subcategories': 'التصنيفات الفرعية',
    'admin.id': 'المعرف',
    'admin.searchByName': 'بحث بالاسم...',
    'admin.allStatus': 'جميع الحالات',
    'admin.allApprovalStatus': 'جميع حالات الموافقة',
    'admin.noPendingShops': 'لا توجد متاجر معلقة',
    'admin.noShopsFound': 'لا توجد متاجر',
    'admin.noProductsFound': 'لا توجد منتجات',
    'admin.noCategoriesFound': 'لا توجد تصنيفات',
    'admin.noSubcategoriesFound': 'لا توجد تصنيفات فرعية',
    'admin.title': 'العنوان',
    'admin.approval': 'الموافقة',
    'admin.assignRole': 'تعيين دور للمستخدم',
    'admin.enterUsername': 'أدخل اسم المستخدم',
    'admin.selectRole': 'اختر الدور',
    'admin.addCategory': 'إضافة تصنيف جديد',
    'admin.editCategory': 'تعديل التصنيف',
    'admin.categoryName': 'اسم التصنيف',
    'admin.imageUrl': 'رابط الصورة',
    'admin.addSubcategory': 'إضافة تصنيف فرعي جديد',
    'admin.editSubcategory': 'تعديل التصنيف الفرعي',
    'admin.subcategoryName': 'اسم التصنيف الفرعي',
    'admin.categoryId': 'معرف التصنيف',
    'admin.rejectShop': 'رفض المتجر',
    'admin.rejectProduct': 'رفض المنتج',
    'admin.rejectionMessage': 'رسالة الرفض',
    'admin.enterRejectionReason': 'أدخل سبب الرفض...',
    // Cart
    'cart.title': 'سلة التسوق',
    'cart.items': 'عناصر',
    'cart.empty': 'السلة فارغة',
    'cart.emptyMessage': 'لم تضف أي منتجات إلى سلتك بعد',
    'cart.continueShopping': 'متابعة التسوق',
    'cart.subtotal': 'المجموع الفرعي',
    'cart.total': 'الإجمالي',
    'cart.checkout': 'إتمام الشراء',
    'cart.clearCart': 'إفراغ السلة',
    'cart.remove': 'إزالة',
    'cart.orderSummary': 'ملخص الطلب',
    'cart.addToCart': 'أضف للسلة',
    'cart.addedToCart': 'تمت الإضافة للسلة'
  },
  en: {
    // Common
    'app.name': 'HandMade',
    'common.loading': 'Loading...',
    'common.retry': 'Retry',
    'common.tryAgain': 'Try again',
    'common.cancel': 'Cancel',
    'common.save': 'Save',
    'common.edit': 'Edit',
    'common.delete': 'Delete',
    'common.add': 'Add',
    'common.create': 'Create',
    'common.update': 'Update',
    'common.search': 'Search',
    'common.apply': 'Apply',
    'common.back': 'Back',
    'common.skip': 'Skip',
    'common.yes': 'Yes',
    'common.no': 'No',
    'common.all': 'All',
    'common.active': 'Active',
    'common.inactive': 'Inactive',
    'common.pending': 'Pending',
    'common.approved': 'Approved',
    'common.rejected': 'Rejected',
    'common.name': 'Name',
    'common.description': 'Description',
    'common.price': 'Price',
    'common.status': 'Status',
    'common.actions': 'Actions',
    'common.image': 'Image',
    'common.noResults': 'No results found',
    // Navigation
    'nav.home': 'Home',
    'nav.categories': 'Categories',
    'nav.login': 'Login',
    'nav.logout': 'Logout',
    'nav.register': 'Register',
    'nav.language': 'Language',
    'nav.profile': 'Profile',
    'nav.myShop': 'My Shop',
    'nav.adminDashboard': 'Admin Dashboard',
    // Footer
    'footer.tagline': 'A marketplace for Fine Arts students to sell their work at student-friendly prices.',
    'footer.usefulLinks': 'Useful Links',
    'footer.customerServices': 'Customer Services',
    'footer.orderTracking': 'Order Tracking',
    'footer.wishlist': 'Wishlist',
    'footer.payment': 'Payment',
    'footer.aboutUs': 'About Us',
    'footer.contactUs': 'Contact Us',
    'footer.ourBlog': 'Our Blog',
    'footer.faqs': 'FAQs',
    'footer.newsletter': 'Signup Our Newsletter',
    'footer.newsletterText': 'Get Update Information or Promotions',
    'footer.subscribe': 'Subscribe',
    'footer.emailUs': 'Email Us',
    'footer.callUs': 'Call Us',
    'footer.ourLocation': 'Our Location',
    'footer.copyright': 'Copyright © All Right Reserved',
    // Landing
    'landing.hero.title': 'Show your artwork… and reach the right buyer',
    'landing.hero.subtitle': 'A platform that helps Fine Arts students sell paintings, sculptures, and handmade pieces instead of storing them at home — at fair, gallery-free prices.',
    'landing.hero.cta.primary': 'Explore categories',
    'landing.hero.cta.secondary': 'About',
    'landing.categories.title': 'Categories',
    'landing.categories.subtitle': 'Pick a category to see its sub-categories.',
    'landing.sidebar.categories': 'Categories',
    'landing.sidebar.subscribe.title': 'Subscribe for updates',
    'landing.sidebar.subscribe.text': 'Get updates when new student artworks are added.',
    'landing.sidebar.subscribe.placeholder': 'Your email address',
    'landing.sidebar.subscribe.button': 'Subscribe',
    'landing.searchPlaceholder': 'Search photos and illustrations',
    'landing.contributeTitle': 'Contribute your first photo',
    'landing.festiveTitle': 'Festive Frames & Copy Space',
    'landing.images': 'images',
    'landing.noProducts': 'No products available.',
    'landing.how.title': 'How it works',
    'landing.how.step1.title': 'Upload your work',
    'landing.how.step1.text': 'Add photos and details (size, material, price).',
    'landing.how.step2.title': 'Get discovered',
    'landing.how.step2.text': 'Buyers browse via categories and sub-categories.',
    'landing.how.step3.title': 'Sell at a fair price',
    'landing.how.step3.text': 'Student-friendly pricing, cheaper than galleries.',
    'landing.value.title': 'Why Unlimited?',
    'landing.value.bullet1': 'Helps students avoid storing work at home.',
    'landing.value.bullet2': 'Fair pricing for students and buyers.',
    'landing.value.bullet3': 'Clear categories for easy browsing.',
    'landing.cta.title': 'Start with categories',
    'landing.cta.text': 'Choose a category and browse its sub-categories.',
    'landing.cta.button': 'View categories',
    // Subcategories
    'subcategories.title': 'Sub-categories',
    'subcategories.back': 'Back',
    'subcategories.empty': 'No sub-categories available for this category yet.',
    // Auth
    'auth.login': 'Login',
    'auth.register': 'Register',
    'auth.logout': 'Logout',
    'auth.createAccount': 'Unlimited - Create Account',
    'auth.joinCommunity': 'Join Unlimited community today',
    'auth.welcomeBack': 'Welcome back.',
    'auth.loggingIn': 'Logging in...',
    'auth.registering': 'Registering...',
    'auth.username': 'Username',
    'auth.email': 'Email',
    'auth.phone': 'Phone Number',
    'auth.password': 'Password',
    'auth.confirmPassword': 'Confirm Password',
    'auth.usernameRequired': 'Username is required.',
    'auth.usernameMinLength': 'Username must be at least 3 characters.',
    'auth.emailRequired': 'Email is required.',
    'auth.emailInvalid': 'Please enter a valid email address.',
    'auth.phoneRequired': 'Phone number is required.',
    'auth.phoneInvalid': 'Please enter a valid phone number (digits only).',
    'auth.passwordRequired': 'Password is required.',
    'auth.passwordMinLength': 'Password must be at least 8 characters.',
    'auth.passwordPattern': 'Password must include uppercase, lowercase, a number and a special character.',
    'auth.passwordHint': 'Hint: 8+ characters, upper & lower case, number and symbol.',
    'auth.confirmPasswordRequired': 'Confirm password is required.',
    'auth.passwordMismatch': 'Passwords do not match.',
    'auth.forgotPassword': 'Forgot your password?',
    'auth.alreadyHaveAccount': 'Already have an account?',
    'auth.dontHaveAccount': "Don't have an account?",
    'auth.join': 'Join',
    // Select role
    'role.chooseRole': 'Choose your role',
    'role.selectBuyerOrSeller': 'Select whether you are a buyer or a seller',
    'role.buyer': 'Buyer',
    'role.seller': 'Seller',
    'role.buyerDesc': 'Browse and buy art',
    'role.sellerDesc': 'Sell your art and create a shop',
    'role.skipForNow': 'Skip for now',
    // Shop
    'shop.createShop': 'Create Shop',
    'shop.createYourShop': 'Create Your Shop',
    'shop.noShopYet': "You haven't created a shop yet. Start now and share your creations with the world!",
    'shop.startNow': 'Start now',
    'shop.createNewShop': 'Create New Shop',
    'shop.pendingApproval': 'Your Shop is Pending Approval',
    'shop.pendingMessage': "Thank you for creating your shop! It's currently being reviewed by our admin team. You'll be notified once it's approved.",
    'shop.shopDetails': 'Shop Details',
    'shop.shopName': 'Shop Name',
    'shop.shopImage': 'Shop Image',
    'shop.rejected': 'Your Shop Was Rejected',
    'shop.rejectionReason': 'Reason for Rejection:',
    'shop.noReasonProvided': 'No reason provided.',
    'shop.rejectionHelp': 'You can edit your shop details and resubmit for approval.',
    'shop.editShop': 'Edit Shop',
    'shop.saveChanges': 'Save Changes',
    'shop.creating': 'Creating...',
    'shop.manageShop': 'Manage Your Shop',
    'shop.manageCategories': 'Manage Categories',
    'shop.manageSubCategories': 'Manage Sub-Categories',
    // Products
    'product.myProducts': 'My Products',
    'product.addProduct': 'Add Product',
    'product.addNewProduct': 'Add New Product',
    'product.addFirstProduct': 'Add Your First Product',
    'product.noProducts': 'No products in this section',
    'product.totalProducts': 'Total Products',
    'product.activeProducts': 'Active Products',
    'product.inactiveProducts': 'Inactive Products',
    'product.pendingApproval': 'Pending Approval',
    'product.productTitle': 'Product Title',
    'product.enterTitle': 'Enter title',
    'product.category': 'Category',
    'product.selectCategory': 'Select Category',
    'product.subcategory': 'Subcategory',
    'product.selectSubcategory': 'Select Subcategory',
    'product.productImage': 'Product Image',
    'product.adding': 'Adding...',
    // Admin
    'admin.dashboard': 'Admin Dashboard',
    'admin.backToSite': 'Back to Site',
    'admin.pendingShops': 'Pending Shops',
    'admin.allShops': 'All Shops',
    'admin.reviewApplications': 'Review and manage shop applications',
    'admin.allCaughtUp': 'All caught up!',
    'admin.noPendingApps': 'There are no pending shop applications at the moment.',
    'admin.approve': 'Approve',
    'admin.reject': 'Reject',
    'admin.rejectionReason': 'Rejection Reason',
    'admin.sendAndReject': 'Send & Reject',
    'admin.rejectPlaceholder': 'Write the rejection reason; this will be sent to the seller',
    'admin.owner': 'Owner',
    'admin.noDescription': 'No description provided.',
    'admin.loadingRequests': 'Loading requests...',
    'admin.shops': 'Shops',
    'admin.products': 'Products',
    'admin.users': 'Users',
    'admin.categories': 'Categories',
    'admin.subcategories': 'SubCategories',
    'admin.id': 'ID',
    'admin.searchByName': 'Search by name...',
    'admin.allStatus': 'All Status',
    'admin.allApprovalStatus': 'All Approval Status',
    'admin.noPendingShops': 'No pending shops',
    'admin.noShopsFound': 'No shops found',
    'admin.noProductsFound': 'No products found',
    'admin.noCategoriesFound': 'No categories found',
    'admin.noSubcategoriesFound': 'No subcategories found',
    'admin.title': 'Title',
    'admin.approval': 'Approval',
    'admin.assignRole': 'Assign Role to User',
    'admin.enterUsername': 'Enter username',
    'admin.selectRole': 'Select Role',
    'admin.addCategory': 'Add New Category',
    'admin.editCategory': 'Edit Category',
    'admin.categoryName': 'Category name',
    'admin.imageUrl': 'Image URL',
    'admin.addSubcategory': 'Add New SubCategory',
    'admin.editSubcategory': 'Edit SubCategory',
    'admin.subcategoryName': 'SubCategory name',
    'admin.categoryId': 'Category ID',
    'admin.rejectShop': 'Reject Shop',
    'admin.rejectProduct': 'Reject Product',
    'admin.rejectionMessage': 'Rejection Message',
    'admin.enterRejectionReason': 'Enter reason for rejection...',
    // Cart
    'cart.title': 'Shopping Cart',
    'cart.items': 'items',
    'cart.empty': 'Your cart is empty',
    'cart.emptyMessage': "You haven't added any products to your cart yet",
    'cart.continueShopping': 'Continue Shopping',
    'cart.subtotal': 'Subtotal',
    'cart.total': 'Total',
    'cart.checkout': 'Checkout',
    'cart.clearCart': 'Clear Cart',
    'cart.remove': 'Remove',
    'cart.orderSummary': 'Order Summary',
    'cart.addToCart': 'Add to Cart',
    'cart.addedToCart': 'Added to Cart'
  }
};
