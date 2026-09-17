import { Routes } from '@angular/router';
import { RoleGuard, SelectRoleGuard } from './core/auth/role.guard';

export const appRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/landing/landing.page').then((m) => m.LandingPage),
    title: 'Unlimited'
  },
  {
    path: 'products',
    loadComponent: () => import('./features/products/products.page').then((m) => m.ProductsPage),
    title: 'Products - Unlimited'
  },
  {
    path: 'product/:productId',
    loadComponent: () =>
      import('./features/product-details/product-details.page').then((m) => m.ProductDetailsPage),
    title: 'Product - Unlimited'
  },
  {
    path: 'category/:categoryId',
    loadComponent: () => import('./features/category/category.page').then((m) => m.CategoryPage),
    title: 'Category - Unlimited'
  },
  {
    path: 'categories/:categoryId/subcategories',
    loadComponent: () =>
      import('./features/subcategories/subcategories.page').then((m) => m.SubCategoriesPage),
    title: 'Unlimited'
  },
  {
    path: 'login',
    loadComponent: () => import('./features/login/login.page').then((m) => m.LoginPage),
    title: 'Login - Unlimited'
  },
  {
    path: 'register',
    loadComponent: () => import('./features/register/register.page').then((m) => m.RegisterPage),
    title: 'Register - Unlimited'
  },
  {
    path: 'select-role',
    loadComponent: () => import('./features/select-role/select-role.page').then((m) => m.SelectRolePage),
    title: 'Choose Role - Unlimited',
    canActivate: [SelectRoleGuard]
  },
  {
    path: 'addresses',
    loadComponent: () => import('./features/addresses/addresses.page').then((m) => m.AddressesPage),
    title: 'My Addresses - Unlimited',
    canActivate: [RoleGuard]
  },
  {
    path: 'cart',
    loadComponent: () => import('./features/cart/cart.page').then((m) => m.CartPage),
    title: 'Cart - Unlimited',
    canActivate: [RoleGuard],
    data: { roles: ['Client'] }
  },
  {
    path: 'checkout',
    loadComponent: () => import('./features/checkout/checkout.page').then((m) => m.CheckoutPage),
    title: 'Checkout - Unlimited',
    canActivate: [RoleGuard],
    data: { roles: ['Client'] }
  },
  {
    path: 'orders',
    loadComponent: () => import('./features/orders/buyer-orders/buyer-orders.page').then((m) => m.BuyerOrdersPage),
    title: 'My Orders - Unlimited',
    canActivate: [RoleGuard],
    data: { roles: ['Client'] }
  },
  {
    path: 'orders/:orderId',
    loadComponent: () =>
      import('./features/orders/order-details/order-details.page').then((m) => m.OrderDetailsPage),
    title: 'Order Details - Unlimited',
    canActivate: [RoleGuard],
    data: { roles: ['Client'] }
  },
  {
    path: 'create-shop',
    loadComponent: () => import('./features/create-shop/create-shop.page').then((m) => m.CreateShopPage),
    title: 'Create Shop - Unlimited',
    canActivate: [RoleGuard],
    data: { roles: ['Artist'] }
  },
  {
    path: 'my-shop',
    loadComponent: () => import('./features/my-shop/my-shop.page').then((m) => m.MyShopPage),
    title: 'My Shop - Unlimited',
    canActivate: [RoleGuard],
    data: { roles: ['Artist'] }
  },
  {
    path: 'my-shop/add-product',
    loadComponent: () => import('./features/my-shop/add-product.page').then((m) => m.AddProductPage),
    title: 'Add Product - Unlimited',
    canActivate: [RoleGuard],
    data: { roles: ['Artist'] }
  },
  {
    path: 'my-shop/orders',
    loadComponent: () => import('./features/my-shop/artist-orders.page').then((m) => m.ArtistOrdersPage),
    title: 'My Shop Orders - Unlimited',
    canActivate: [RoleGuard],
    data: { roles: ['Artist'] }
  },
  {
    path: 'admin',
    loadComponent: () => import('./features/admin-dashboard/admin-dashboard.page').then((m) => m.AdminDashboardPage),
    title: 'Admin Dashboard - Unlimited',
    canActivate: [RoleGuard],
    data: { roles: ['Admin', 'SuperAdmin', 'Super Admin'] }
  },
  {
    path: 'admin/products/pending',
    loadComponent: () => import('./features/admin-products-pending/admin-products-pending.page').then((m) => m.AdminProductsPendingPage),
    title: 'Pending Products - Admin',
    canActivate: [RoleGuard],
    data: { roles: ['Admin', 'SuperAdmin', 'Super Admin'] }
  },
  {
    path: '**',
    redirectTo: ''
  }
];
