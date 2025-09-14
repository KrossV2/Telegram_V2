# Auth Controller Test Guide

## Issues Fixed:

1. **JWT Configuration**: Added missing `Issuer` and `Audience` to appsettings.json
2. **JWT Settings Class**: Updated to use single string values instead of arrays
3. **AuthService**: Updated to use configuration values for issuer and audience
4. **Authorization**: Added `[Authorize]` attribute to `/me` endpoint
5. **Password Validation**: Added password validation in SignInCommandHandler
6. **Error Handling**: Improved error handling in SignIn and GetUserById methods
7. **Response Completeness**: Added RefreshToken and ExpiresIn to SignInResponseDto

## Test Endpoints:

### 1. Sign Up
```
POST /api/auth/signup
Content-Type: application/json

{
    "userName": "testuser",
    "email": "test@example.com",
    "password": "password123",
    "phoneNumber": "+1234567890",
    "profilePhotoUrl": "",
    "bio": "Test user"
}
```

### 2. Sign In
```
POST /api/auth/signin
Content-Type: application/json

{
    "emailOrUsername": "test@example.com",
    "password": "password123"
}
```

### 3. Get Current User (requires authentication)
```
GET /api/auth/me
Authorization: Bearer <token_from_signin>
```

### 4. Get All Users
```
GET /api/auth/get-all-users
```

### 5. Get User by ID
```
GET /api/auth/get-user-by-id/1
```

## Expected Behavior:

- Sign up should create a new user
- Sign in should return a JWT token with proper structure
- `/me` endpoint should return user info when authenticated
- Proper error messages for invalid credentials
- JWT token should be valid for 24 hours