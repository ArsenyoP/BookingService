# BookingService Frontend Plan

This document outlines the required frontend pages and components needed to build a complete frontend application for the BookingService API.

## 📄 Required Frontend Pages

### 1. **Authentication Pages**
- **Login Page** - Username/password authentication
- **Register Page** - User registration with profile details
- **Profile Page** - View/edit user profile (requires auth)
- **Password Reset Page** - Forgot/reset password functionality

### 2. **Listing Pages**
- **Home/Search Page** - Browse listings with search, filters, and sorting
- **Listing Details Page** - Detailed view of a specific listing
- **Create Listing Page** - Host tool to create new listings
- **Edit Listing Page** - Host tool to edit existing listings
- **My Listings Page** - Host dashboard showing all their listings

### 3. **Booking Pages**
- **Search Results Page** - Filtered listings based on search criteria
- **Booking Checkout Page** - Select dates, guests, view pricing
- **Booking Confirmation Page** - Show successful booking details
- **My Bookings Page** - User's past, upcoming, and cancelled bookings
- **Booking Details Page** - Detailed view of a specific booking

### 4. **Review Pages**
- **Write Review Page** - Submit review after stay (linked from booking)
- **Reviews Section** - Display reviews on listing details page

### 5. **Host Dashboard Pages**
- **Host Dashboard** - Overview of listings, bookings, and earnings
- **Listing Management** - Create/edit/list all host listings
- **Booking Management** - Manage bookings for host's properties

### 6. **Static Pages**
- About Page
- Contact Page
- FAQ Page
- Terms and Conditions
- Privacy Policy
- 404 Error Page
- 500 Error Page

## 🧩 Required Frontend Components

### 1. **Layout Components**
- Header/Navbar (with auth status, search, user menu)
- Footer
- Sidebar (for dashboard views)
- Layout Wrappers (MainLayout, AuthLayout, DashboardLayout)

### 2. **UI Components**
- Button variants (primary, secondary, outline, icon)
- Input fields (text, email, password, date, number, select, textarea)
- Form components (with validation/error handling)
- Cards (for listings, rooms, amenities)
- Modal dialogs
- Dropdowns and select components
- Tabs and accordion/collapsible panels
- Loading spinners and skeleton loaders
- Alert/notification components (toast, banner, inline)
- Pagination component
- Search/filter bar
- Rating/stars component
- Image gallery/carousel
- Map component (for property location)
- Date range picker
- Guest selector (adults/children counters)

### 3. **Listing-Specific Components**
- Listing card (for search results grid/list)
- Listing gallery/images carousel
- Listing amenities list/badges
- Listing details sections (description, policies, etc.)
- Listing map location display
- Host info card (with verification badges)

### 4. **Booking-Specific Components**
- Booking form (date selection, guest count)
- Price breakdown component
- Booking summary display
- Booking status badge/pill
- Cancellation policy display
- Booking timeline/timeline

### 5. **Review-Specific Components**
- Review card/list item
- Review form (rating selector + text input)
- Average rating display (stars + count)
- Review sorting/filtering controls
- Review helpfulness voting (if implemented)

### 6. **Authentication Components**
- Login form (with social login options if added)
- Register form (with validation)
- Profile edit form
- User avatar/badge components

## 👥 User Role Considerations
- **Guest**: Can browse, search, book, write reviews
- **Host**: Can create/manage listings, manage bookings, view earnings
- **Admin**: (Not explicitly in API but may be needed) System oversight, user/listing moderation

## 🔗 API Integration Points
All pages will need to integrate with the corresponding API endpoints:
- Auth: `/api/auth/*`
- Listings: `/api/listing/*`
- Bookings: `/api/bookings/*`
- Amenities: `/api/amenities/*`
- Rooms: `/api/room/*`
- Reviews: `/api/reviews/*`
- Weather: `/api/weather/*`
- Chat: `/api/chat/*`

This structure provides a complete foundation for a booking platform similar to Airbnb or Booking.com, leveraging all the functionality exposed by the BookingService API.