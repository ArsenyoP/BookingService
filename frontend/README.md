# BookingService Frontend

This is the frontend implementation for the BookingService API, built with HTML, CSS, and TypeScript.

## 📁 Directory Structure

```
frontend/
├── pages/          # HTML pages
├── components/     # Reusable components (to be implemented)
├── assets/         # Images, icons, and other static assets
├── css/            # Stylesheets
├── js/             # Compiled JavaScript (from TypeScript)
├── tsconfig.json   # TypeScript configuration
└── README.md       # This file
```

## 🛠️ Setup & Development

### Prerequisites
- Node.js (for TypeScript compilation)
- TypeScript compiler (`tsc`)

### Installation
1. Install TypeScript globally (if not already installed):
   ```bash
   npm install -g typescript
   ```

### Development Workflow
1. Write TypeScript code in `.ts` files (currently `js/main.ts`)
2. Compile to JavaScript:
   ```bash
   tsc
   ```
   This will compile `.ts` files to `.js` files in the `js/` directory as specified in `tsconfig.json`.
3. Reference the compiled `.js` files in your HTML pages (already done).

### Available Pages
- `index.html` - Home page with search and featured listings
- `listing-details.html` - Detailed view of a specific listing
- `login.html` - User login page
- `register.html` - User registration page
- `create-listing.html` - Host dashboard for creating new listings
- `my-bookings.html` - User's booking history

## 🧩 Features Implemented

### UI Components
- Responsive layout with header, footer, and main content areas
- Hero section with search form
- Listing cards grid with images, titles, locations, ratings, and prices
- Listing detail page with gallery, amenities, host info, and reviews
- Authentication forms (login, register)
- Host dashboard for creating listings
- Booking history page
- Interactive star rating system for reviews
- Thumbnail navigation for image galleries
- Mobile-responsive design

### Styling
- Clean, modern design with consistent color scheme
- Hover effects and transitions
- Responsive breakpoints for mobile devices
- Card-based layout for listings and bookings
- Form validation styling

### Interactivity (TypeScript)
- Star rating selection for reviews
- Thumbnail click to change main image
- Form submission handling (demo alerts)
- DOMContentLoaded event handling

## 🔗 API Integration Points

The frontend is designed to integrate with the BookingService API endpoints:

- **Authentication**: `/api/auth/*` (login, register)
- **Listings**: `/api/listing/*` (get listings, get by ID, create, update, delete)
- **Bookings**: `/api/bookings/*` (get bookings, create, cancel, confirm)
- **Amenities**: `/api/amenities/*` (get amenities, create)
- **Rooms**: `/api/room/*` (get rooms, create)
- **Reviews**: `/api/reviews/*` (get reviews, create, update, delete)
- **Weather**: `/api/weather/*` (get weather by city)
- **Chat**: `/api/chat/*` (search functionality)

## 🚀 Future Enhancements

1. **Framework Migration**: Consider migrating to React/Vue/Angular for better state management
2. **State Management**: Implement proper state management (Redux, Vuex, etc.)
3. **Routing**: Add client-side routing for SPA experience
4. **API Service Layer**: Create TypeScript services for API communication
5. **Authentication**: Implement JWT token handling and refresh
6. **Form Validation**: Add comprehensive client-side validation
7. **Loading States**: Add skeletons and loading spinners
8. **Error Handling**: Implement proper error display and retry mechanisms
9. **Accessibility**: Improve ARIA labels and keyboard navigation
10. **Testing**: Add unit and integration tests

## 📱 Responsiveness

The design is mobile-first and responsive:
- Mobile: Single column layout, full-width elements
- Tablet: Two-column layouts where appropriate
- Desktop: Multi-column layouts with sidebar options

## 🎨 Design System

- **Primary Color**: #ff6b6b (coral)
- **Neutrals**: #333, #555, #777, #ecf0f1, #f8f9fa
- **Accent**: #f1c40f (gold for ratings)
- **Font**: System UI stack (Segoe UI, Tahoma, etc.)

---
*Note: This is a static frontend demo. For full functionality, API endpoints need to be implemented and connected.*