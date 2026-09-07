# Booking Service API Documentation

## Auth Controller
Base URL: `/api/auth`

### Register a new user
- **URL**: `/register`
- **Method**: POST
- **Request Body**:
  ```json
  {
    "registerDto": {
      "userName": "string",
      "firstName": "string",
      "lastName": "string",
      "email": "string",
      "password": "string",
      "dateOfBirth": "YYYY-MM-DD"
    },
    "role": "string"
  }
  ```
- **Responses**:
  - 200 OK: 
    ```json
    {
      "userDto": {
        "userName": "string",
        "firstName": "string",
        "lastName": "string",
        "email": "string",
        "jwtToken": "string",
        "refreshToke": "string"
      },
      "refreshToken": "string"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Login
- **URL**: `/login`
- **Method**: POST
- **Request Body**:
  ```json
  {
    "loginDto": {
      "userName": "string",
      "password": "string"
    }
  }
  ```
- **Responses**:
  - 200 OK: 
    ```json
    {
      "userDto": {
        "userName": "string",
        "firstName": "string",
        "lastName": "string",
        "email": "string",
        "jwtToken": "string",
        "refreshToke": "string"
      },
      "refreshToken": "string"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Refresh Token Login
- **URL**: `/login/refreshToken`
- **Method**: POST
- **Request Body**:
  ```json
  {
    "refreshToken": "string"
  }
  ```
- **Responses**:
  - 200 OK: 
    ```json
    {
      "accessToken": "string",
      "refreshToken": "string"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

## RefreshToken Controller
Base URL: `/api/refresoToken`  <!-- Note: typo in the route -->

### Invalidate Refresh Token
- **URL**: `/invalidate`
- **Method**: DELETE
- **Query Parameters**:
  - `userId` (Guid, required): The user ID whose refresh token should be invalidated
- **Responses**:
  - 200 OK: 
    ```json
    {
      "value": true
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

## Booking Controller
Base URL: `/api/bookings`

### Get All Bookings
- **URL**: (empty, i.e., `/api/bookings`)
- **Method**: GET
- **Query Parameters**:
  - `page` (integer, optional, default: 1): The page number
  - `pageSize` (integer, optional, default: 10): The number of items per page
- **Responses**:
  - 200 OK: 
    ```json
    [
      {
        "id": "guid",
        "roomId": "guid",
        "guestId": "guid",
        "startDate": "YYYY-MM-DDTHH:mm:ss",
        "endDate": "YYYY-MM-DDTHH:mm:ss",
        "totalNights": "integer",
        "pricePerNight": "decimal",
        "totalPrice": "decimal",
        "adultsCount": "integer",
        "childrenCount": "integer",
        "status": "string",
        "roomTitle": "string",
        "firstName": "string",
        "lastName": "string"
      }
    ]
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Get Booking by ID
- **URL**: `/{id:guid}`
- **Method**: GET
- **URL Parameters**:
  - `id` (Guid, required): The booking ID
- **Responses**:
  - 200 OK: 
    ```json
    {
      "id": "guid",
      "roomId": "guid",
      "guestId": "guid",
      "startDate": "YYYY-MM-DDTHH:mm:ss",
      "endDate": "YYYY-MM-DDTHH:mm:ss",
      "totalNights": "integer",
      "pricePerNight": "decimal",
      "totalPrice": "decimal",
      "adultsCount": "integer",
      "childrenCount": "integer",
      "status": "string",
      "roomTitle": "string",
      "firstName": "string",
      "lastName": "string"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Get Bookings by Room ID
- **URL**: `/room/{id:guid}`
- **Method**: GET
- **URL Parameters**:
  - `id` (Guid, required): The room ID
- **Query Parameters**:
  - `page` (integer, optional, default: 1): The page number
  - `pageSize` (integer, optional, default: 10): The number of items per page
- **Responses**:
  - 200 OK: 
    ```json
    [
      {
        "id": "guid",
        "roomId": "guid",
        "guestId": "guid",
        "startDate": "YYYY-MM-DDTHH:mm:ss",
        "endDate": "YYYY-MM-DDTHH:mm:ss",
        "totalNights": "integer",
        "pricePerNight": "decimal",
        "totalPrice": "decimal",
        "adultsCount": "integer",
        "childrenCount": "integer",
        "status": "string",
        "roomTitle": "string",
        "firstName": "string",
        "lastName": "string"
      }
    ]
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Get Bookings by User ID
- **URL**: `/user/{id:guid}`
- **Method**: GET
- **URL Parameters**:
  - `id` (Guid, required): The user ID
- **Query Parameters**:
  - `page` (integer, optional, default: 1): The page number
  - `pageSize` (integer, optional, default: 10): The number of items per page
- **Responses**:
  - 200 OK: 
    ```json
    [
      {
        "id": "guid",
        "roomId": "guid",
        "guestId": "guid",
        "startDate": "YYYY-MM-DDTHH:mm:ss",
        "endDate": "YYYY-MM-DDTHH:mm:ss",
        "totalNights": "integer",
        "pricePerNight": "decimal",
        "totalPrice": "decimal",
        "adultsCount": "integer",
        "childrenCount": "integer",
        "status": "string",
        "roomTitle": "string",
        "firstName": "string",
        "lastName": "string"
      }
    ]
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Check Room Availability
- **URL**: `/roomBool/{id:guid}`
- **Method**: GET
- **URL Parameters**:
  - `id` (Guid, required): The room ID
- **Query Parameters**:
  - `start` (DateOnly, required): The start date of the booking
  - `end` (DateOnly, required): The end date of the booking
- **Responses**:
  - 200 OK: 
    ```json
    {
      "value": true
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Create Booking
- **URL**: (empty, i.e., `/api/bookings`)
- **Method**: POST
- **Request Body**:
  ```json
  {
    "roomId": "guid",
    "startDate": "YYYY-MM-DD",
    "endDate": "YYYY-MM-DD",
    "adultsCount": "integer",
    "childrenCount": "integer"
  }
  ```
- **Responses**:
  - 201 Created: 
    ```json
    {
      "id": "guid",
      "roomId": "guid",
      "guestId": "guid",
      "startDate": "YYYY-MM-DDTHH:mm:ss",
      "endDate": "YYYY-MM-DDTHH:mm:ss",
      "totalNights": "integer",
      "pricePerNight": "decimal",
      "totalPrice": "decimal",
      "adultsCount": "integer",
      "childrenCount": "integer",
      "status": "string",
      "roomTitle": "string",
      "firstName": "string",
      "lastName": "string"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Delete Booking
- **URL**: `/{id:guid}`
- **Method**: DELETE
- **URL Parameters**:
  - `id` (Guid, required): The booking ID
- **Responses**:
  - 200 OK: 
    ```json
    {
      "value": true
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Confirm Booking
- **URL**: `/confirm`
- **Method**: GET
- **Query Parameters**:
  - `token` (string, required): The confirmation token
- **Responses**:
  - 200 OK: 
    ```json
    {}
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Cancel Booking
- **URL**: `/cancel/{bookingId:guid}`
- **Method**: POST
- **URL Parameters**:
  - `bookingId` (Guid, required): The booking ID to cancel
- **Responses**:
  - 200 OK: 
    ```json
    {}
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

## Amenity Controller
Base URL: `/api/amenities`

### Get All Amenities
- **URL**: (empty, i.e., `/api/amenities`)
- **Method**: GET
- **Query Parameters**:
  - `page` (integer, optional, default: 1): The page number
  - `pageSize` (integer, optional, default: 10): The number of items per page
- **Responses**:
  - 200 OK: 
    ```json
    [
      {
        "amenityId": "guid",
        "name": "string",
        "category": "string"  // Note: AmenityCategory is an enum, so string representation
      }
    ]
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Create Amenity
- **URL**: (empty, i.e., `/api/amenities`)
- **Method**: POST
- **Request Body**:
  ```json
  {
    "name": "string",
    "category": "string"
  }
  ```
- **Responses**:
  - 201 Created: 
    ```json
    {
      "amenityId": "guid",
      "name": "string",
      "category": "string"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Add Amenity to Room
- **URL**: `/addToRoom`
- **Method**: POST
- **Request Body**:
  ```json
  {
    "amenityId": "guid",
    "roomId": "guid"
  }
  ```
- **Responses**:
  - 200 OK: 
    ```json
    {
      "amenityId": "guid",
      "name": "string",
      "category": "string"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Remove Amenity from Room
- **URL**: `/removeFromRoom`
- **Method**: POST
- **Request Body**:
  ```json
  {
    "amenityId": "guid",
    "roomId": "guid"
  }
  ```
- **Responses**:
  - 200 OK: 
    ```json
    {
      "amenityId": "guid",
      "name": "string",
      "category": "string"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Add Amenity to Listing
- **URL**: `/addToListing`
- **Method**: POST
- **Request Body**:
  ```json
  {
    "amenityId": "guid",
    "listingId": "guid"
  }
  ```
- **Responses**:
  - 200 OK: 
    ```json
    {
      "amenityId": "guid",
      "name": "string",
      "category": "string"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Remove Amenity from Listing
- **URL**: `/RemoveFromListing`  <!-- Note: case-sensitive as in the code -->
- **Method**: POST
- **Request Body**:
  ```json
  {
    "amenityId": "guid",
    "listingId": "guid"
  }
  ```
- **Responses**:
  - 200 OK: 
    ```json
    {
      "amenityId": "guid",
      "name": "string",
      "category": "string"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Delete Amenity
- **URL**: `/{name}`  <!-- Note: the route uses {name} -->
- **Method**: DELETE
- **URL Parameters**:
  - `name` (string, required): The name of the amenity to delete
- **Responses**:
  - 200 OK: 
    ```json
    {
      "value": true
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

## Listing Controller
Base URL: `/api/listing`

### Get All Listings
- **URL**: (empty, i.e., `/api/listing`)
- **Method**: GET
- **Query Parameters**:
  - The controller uses a `ListingQueryObject` which we don't have the definition for. We'll note that it accepts query parameters for filtering, but without the definition we cannot specify them.
  - For simplicity, we'll note that it supports pagination and filtering (details in the ListingQueryObject).
- **Responses**:
  - 200 OK: 
    ```json
    [
      {
        "id": "guid",
        "title": "string",
        "description": "string",
        "address": "string",
        "city": "string",
        "country": "string",
        "pricePerNight": "decimal",
        "maxGuests": "integer",
        "rating": "decimal"
      }
    ]
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Get Listing by ID
- **URL**: `/{id:guid}`
- **Method**: GET
- **URL Parameters**:
  - `id` (Guid, required): The listing ID
- **Responses**:
  - 200 OK: 
    ```json
    {
      "id": "guid",
      "title": "string",
      "description": "string",
      "address": "string",
      "city": "string",
      "country": "string",
      "pricePerNight": "decimal",
      "maxGuests": "integer",
      "rating": "decimal"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Create Listing
- **URL**: (empty, i.e., `/api/listing`)
- **Method**: POST
- **Request Body**:
  ```json
  {
    "title": "string",
    "description": "string",
    "address": "string",
    "city": "string",
    "country": "string",
    "pricePerNight": "decimal",
    "maxGuests": "integer"
  }
  ```
- **Responses**:
  - 201 Created: 
    ```json
    {
      "id": "guid",
      "title": "string",
      "description": "string",
      "address": "string",
      "city": "string",
      "country": "string",
      "pricePerNight": "decimal",
      "maxGuests": "integer",
      "rating": "decimal"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Delete Listing
- **URL**: `/{id:guid}`
- **Method**: DELETE
- **URL Parameters**:
  - `id` (Guid, required): The listing ID
- **Responses**:
  - 200 OK: 
    ```json
    {
      "value": true
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

## Review Controller
Base URL: `/api/reviews`

### Create Review
- **URL**: (empty, i.e., `/api/reviews`)
- **Method**: POST
- **Request Body**:
  ```json
  {
    "rating": "integer",
    "comment": "string"
  }
  ```
- **Responses**:
  - 200 OK: 
    ```json
    {
      "id": "guid",
      "targetId": "guid",
      "userId": "guid",
      "rating": "integer",
      "comment": "string",
      "createdAt": "YYYY-MM-DDTHH:mm:ss",
      "firstName": "string",
      "lastName": "string"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Update Review
- **URL**: `/edit/{targetId:guid}`
- **Method**: PATCH
- **URL Parameters**:
  - `targetId` (Guid, required): The ID of the target (listing, room, etc.) being reviewed
- **Request Body**:
  ```json
  {
    "rating": "integer",
    "comment": "string"
  }
  ```
- **Responses**:
  - 200 OK: 
    ```json
    {
      "id": "guid",
      "targetId": "guid",
      "userId": "guid",
      "rating": "integer",
      "comment": "string",
      "createdAt": "YYYY-MM-DDTHH:mm:ss",
      "firstName": "string",
      "lastName": "string"
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Get All Reviews
- **URL**: (empty, i.e., `/api/reviews`)
- **Method**: GET
- **Query Parameters**:
  - `page` (integer, optional, default: 1): The page number
  - `pageSize` (integer, optional, default: 10): The number of items per page
- **Responses**:
  - 200 OK: 
    ```json
    [
      {
        "id": "guid",
        "targetId": "guid",
        "userId": "guid",
        "rating": "integer",
        "comment": "string",
        "createdAt": "YYYY-MM-DDTHH:mm:ss",
        "firstName": "string",
        "lastName": "string"
      }
    ]
    ```
  - 404 Not Found: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Get Reviews by Target ID
- **URL**: `/{targetId:guid}`
- **Method**: GET
- **URL Parameters**:
  - `targetId` (Guid, required): The ID of the target (listing, room, etc.)
- **Query Parameters**:
  - `page` (integer, optional, default: 1): The page number
  - `pageSize` (integer, optional, default: 10): The number of items per page
- **Responses**:
  - 200 OK: 
    ```json
    [
      {
        "id": "guid",
        "targetId": "guid",
        "userId": "guid",
        "rating": "integer",
        "comment": "string",
        "createdAt": "YYYY-MM-DDTHH:mm:ss",
        "firstName": "string",
        "lastName": "string"
      }
    ]
    ```
  - 404 Not Found: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Get Review by ID
- **URL**: `/details/{id:guid}`
- **Method**: GET
- **URL Parameters**:
  - `id` (Guid, required): The review ID
- **Responses**:
  - 200 OK: 
    ```json
    {
      "id": "guid",
      "targetId": "guid",
      "userId": "guid",
      "rating": "integer",
      "comment": "string",
      "createdAt": "YYYY-MM-DDTHH:mm:ss",
      "firstName": "string",
      "lastName": "string"
    }
    ```
  - 404 Not Found: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Get Reviews by User ID
- **URL**: `/user/{userId:guid}`
- **Method**: GET
- **URL Parameters**:
  - `userId` (Guid, required): The user ID
- **Query Parameters**:
  - `page` (integer, optional, default: 1): The page number
  - `pageSize` (integer, optional, default: 10): The number of items per page
- **Responses**:
  - 200 OK: 
    ```json
    [
      {
        "id": "guid",
        "targetId": "guid",
        "userId": "guid",
        "rating": "integer",
        "comment": "string",
        "createdAt": "YYYY-MM-DDTHH:mm:ss",
        "firstName": "string",
        "lastName": "string"
      }
    ]
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Delete Review
- **URL**: `/delete/{targetId:guid}`
- **Method**: DELETE
- **URL Parameters**:
  - `targetId` (Guid, required): The ID of the target (listing, room, etc.) being reviewed
- **Responses**:
  - 200 OK: 
    ```json
    {
      "value": true
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

## Room Controller
Base URL: `/api/room`

### Get All Rooms
- **URL**: (empty, i.e., `/api/room`)
- **Method**: GET
- **Query Parameters**:
  - The controller uses a `RoomQueryObject` which we don't have the definition for. We'll note that it accepts query parameters for filtering, but without the definition we cannot specify them.
  - For simplicity, we'll note that it supports pagination and filtering (details in the RoomQueryObject).
- **Responses**:
  - 200 OK: 
    ```json
    [
      {
        "id": "guid",
        "listingId": "guid",
        "roomNumber": "string",
        "description": "string",
        "pricePerNight": "decimal",
        "maxGuests": "integer",
        "amenities": [
          {
            "amenityId": "guid",
            "name": "string",
            "category": "string"
          }
        ]
      }
    ]
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Get Room by ID
- **URL**: `/{id:guid}`
- **Method**: GET
- **URL Parameters**:
  - `id` (Guid, required): The room ID
- **Responses**:
  - 200 OK: 
    ```json
    {
      "id": "guid",
      "listingId": "guid",
      "roomNumber": "string",
      "description": "string",
      "pricePerNight": "decimal",
      "maxGuests": "integer",
      "amenities": [
        {
          "amenityId": "guid",
          "name": "string",
          "category": "string"
        }
      ]
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Get Room by Listing ID
- **URL**: `/listingId/{id:guid}`
- **Method**: GET
- **URL Parameters**:
  - `id` (Guid, required): The listing ID
- **Query Parameters**:
  - `page` (integer, optional, default: 1): The page number
  - `pageSize` (integer, optional, default: 10): The number of items per page
- **Responses**:
  - 200 OK: 
    ```json
    [
      {
        "id": "guid",
        "listingId": "guid",
        "roomNumber": "string",
        "description": "string",
        "pricePerNight": "decimal",
        "maxGuests": "integer",
        "amenities": [
          {
            "amenityId": "guid",
            "name": "string",
            "category": "string"
          }
        ]
      ]
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Create Room
- **URL**: (empty, i.e., `/api/room`)
- **Method**: POST
- **Request Body**:
  ```json
  {
    "listingId": "guid",
    "roomNumber": "string",
    "description": "string",
    "pricePerNight": "decimal",
    "maxGuests": "integer"
  }
  ```
- **Responses**:
  - 201 Created: 
    ```json
    {
      "id": "guid",
      "listingId": "guid",
      "roomNumber": "string",
      "description": "string",
      "pricePerNight": "decimal",
      "maxGuests": "integer",
      "amenities": [
        {
          "amenityId": "guid",
          "name": "string",
          "category": "string"
        }
      ]
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```

### Delete Room
- **URL**: `/{id:guid}`
- **Method**: DELETE
- **URL Parameters**:
  - `id` (Guid, required): The room ID
- **Responses**:
  - 200 OK: 
    ```json
    {
      "value": true
    }
    ```
  - 400 Bad Request: 
    ```json
    {
      "errors": [
        {
          "message": "string"
        }
      ]
    }
    ```