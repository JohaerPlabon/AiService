# AI Services Platform - Comprehensive Architecture & Project Plan

## Table of Contents
1. [Executive Summary](#executive-summary)
2. [Project Overview](#project-overview)
3. [System Architecture](#system-architecture)
4. [Database Design](#database-design)
5. [Payment Gateway Integration](#payment-gateway-integration)
6. [AI Services Integration](#ai-services-integration)
7. [Technology Stack](#technology-stack)
8. [Implementation Plan](#implementation-plan)
9. [Security Considerations](#security-considerations)
10. [Deployment Strategy](#deployment-strategy)
11. [Monitoring & Maintenance](#monitoring--maintenance)
12. [Cost Estimation](#cost-estimation)

---

## Executive Summary

### Business Model
Token-based SaaS platform where users purchase tokens and consume them across multiple AI services.

### Key Features
- User registration & authentication
- Token purchasing system with multiple payment gateways
- Multiple AI service integrations (logo design, image generation, audio generation, chatbot)
- Usage tracking and billing
- Admin dashboard for monitoring

### Target Market
- Individuals and businesses needing AI-powered content creation
- Design agencies and marketing companies
- Content creators and social media managers

---

## Project Overview

### Project Name
**AI Services Platform (Token-Based Billing)**

### Tech Stack
- **Backend:** ASP.NET Core 8.0/9.0
- **Frontend:** Blazor Server/WASM or React
- **Database:** SQL Server / PostgreSQL
- **Caching:** Redis
- **Cloud:** Azure / AWS
- **Payment Gateways:** bKash, Nagad, Stripe (for international cards)

### Business Requirements

#### Token Pricing Model
- **Single Token:** 1 TK
- **Bulk Plans:**
  - 60 tokens = 50 TK (17% discount)
  - 120 tokens = 90 TK (25% discount)
  - 300 tokens = 200 TK (33% discount)
  - 1000 tokens = 600 TK (40% discount)

#### Service Token Costs
- **Image Generator:** 5 images = 1 token
- **Logo Design:** 1 logo = 3 tokens
- **Audio Generator:** 1 minute audio = 2 tokens
- **Chatbot:** 100 messages = 1 token

---

## System Architecture

### High-Level Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENT LAYER                              │
├─────────────────────────────────────────────────────────────────┤
│  Web Frontend (Blazor/React)  │  Mobile App (React Native)      │
│  - User Dashboard              │  - Service Access              │
│  - Token Purchase              │  - Usage Tracking              │
│  - Service Interface           │  - Notifications               │
└─────────────┬───────────────────┴───────────────┬────────────────┘
              │                                   │
              └───────────────┬───────────────────┘
                              │ HTTPS/TLS
┌─────────────────────────────┴─────────────────────────────────────┐
│                     API GATEWAY / LOAD BALANCER                  │
│              (Ocelot, YARP, or Cloud Load Balancer)              │
└─────────────────────────────┬─────────────────────────────────────┘
                              │
┌─────────────────────────────┴─────────────────────────────────────┐
│                      APPLICATION LAYER                            │
├──────────────────┬────────────────────────────────────────────────┤
│  User Service    │  Token Service  │  AI Services Orchestrator   │
│  - Auth/AuthZ    │  - Balance      │  - Service Dispatcher       │
│  - Profile       │  - Transactions │  - Cost Calculator          │
│  - KYC           │  - Plans        │  - Usage Tracker            │
├──────────────────┴────────────────────────────────────────────────┤
│  Payment Gateway Service (bKash, Nagad, Stripe)                  │
└─────────────────────────────┬─────────────────────────────────────┘
                              │
┌─────────────────────────────┴─────────────────────────────────────┐
│                      BUSINESS LOGIC LAYER                         │
├──────────────────┬────────────────────────────────────────────────┤
│  Billing Engine  │  AI Service Adapters  │  Notification Service  │
│  - Pricing Rules │  - Image Generator    │  - Email/SMS           │
│  - Token Calc    │  - Logo Designer      │  - In-App Notifications│
│  - Invoicing     │  - Audio Generator    │  - Webhooks            │
│                  │  - Chatbot            │                        │
└──────────────────┴────────────────────────────────────────────────┘
                              │
┌─────────────────────────────┴─────────────────────────────────────┐
│                      DATA ACCESS LAYER                            │
├──────────────────┬────────────────────────────────────────────────┤
│  Repository      │  ORM (EF Core)  │  Caching (Redis)             │
│  Pattern         │  - Database Ops │  - Token Balances            │
└──────────────────┴────────────────────────────────────────────────┘
                              │
┌─────────────────────────────┴─────────────────────────────────────┐
│                      DATA STORAGE LAYER                           │
├──────────────────┬────────────────────────────────────────────────┤
│  Primary DB      │  File Storage   │  Message Queue (RabbitMQ)    │
│  SQL Server/PG   │  Azure Blob/AWS S3  │  - Async Processing       │
│  - Users         │  - Generated Images│  - Payment Notifications   │
│  - Tokens        │  - Audio Files    │  - AI Service Jobs         │
│  - Transactions  │                   │                            │
└──────────────────┴────────────────────────────────────────────────┘
```

### Microservices Architecture

#### Core Services

##### 1. User Authentication Service
**Responsibilities:**
- User registration and login
- JWT token generation and validation
- Password management (reset, change)
- Email verification
- Two-factor authentication (optional)

**Key APIs:**
```
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh-token
POST /api/auth/forgot-password
POST /api/auth/verify-email
```

##### 2. Token Management Service
**Responsibilities:**
- Token balance tracking
- Token purchase processing
- Token deduction for services
- Transaction history
- Token plan management
- Refund handling

**Key APIs:**
```
GET /api/tokens/balance
POST /api/tokens/purchase
GET /api/tokens/plans
GET /api/tokens/transactions
POST /api/tokens/deduct
```

##### 3. Payment Gateway Service
**Responsibilities:**
- Integration with multiple payment gateways
- Payment initiation and processing
- Payment status verification
- Webhook handling
- Transaction reconciliation

**Key APIs:**
```
POST /api/payment/initiate
GET /api/payment/status/{transactionId}
POST /api/payment/webhook/bkash
POST /api/payment/webhook/nagad
POST /api/payment/webhook/stripe
```

##### 4. AI Services Orchestrator
**Responsibilities:**
- Request routing to appropriate AI service
- Cost calculation before execution
- Token balance validation
- Service execution monitoring
- Result aggregation and storage
- Usage tracking

**Key APIs:**
```
POST /api/ai/image-generate
POST /api/ai/logo-design
POST /api/ai/audio-generate
POST /api/ai/chatbot
GET /api/ai/jobs/{jobId}
GET /api/ai/history
```

##### 5. Notification Service
**Responsibilities:**
- Email notifications
- SMS notifications
- In-app notifications
- Push notifications (mobile)
- Webhook notifications

**Key APIs:**
```
POST /api/notifications/send
GET /api/notifications
GET /api/notifications/mark-read/{id}
```

##### 6. Admin Dashboard Service
**Responsibilities:**
- User management
- Revenue tracking
- Service usage analytics
- System monitoring
- Configuration management

**Key APIs:**
```
GET /api/admin/users
GET /api/admin/revenue
GET /api/analytics/usage
GET /api/admin/system-health
```

### Database Design

#### Entity Relationship Diagram

```
┌─────────────────┐       ┌─────────────────┐
│     Users       │       │   UserProfiles  │
├─────────────────┤       ├─────────────────┤
│ Id (PK)         │◄──────┤ UserId (FK)     │
│ Email           │       │ FirstName       │
│ PasswordHash    │       │ LastName        │
│ EmailVerified   │       │ Phone           │
│ IsActive        │       │ AvatarUrl       │
│ CreatedAt       │       │ Address         │
│ UpdatedAt       │       │ DateOfBirth     │
└─────────────────┘       └─────────────────┘
         │
         │ 1:N
         │
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│  TokenBalances  │       │ TokenPlans      │       │  Transactions   │
├─────────────────┤       ├─────────────────┤       ├─────────────────┤
│ Id (PK)         │       │ Id (PK)         │       │ Id (PK)         │
│ UserId (FK)     │◄──────┤ PlanId (FK)     │◄──────┤ TransactionId   │
│ Balance         │       │ TokenAmount     │       │ UserId (FK)     │
│ ReservedBalance │       │ Price           │       │ Amount          │
│ CreatedAt       │       │ IsActive        │       │ Type            │
│ UpdatedAt       │       │ Description     │       │ Status          │
└─────────────────┘       └─────────────────┘       │ PaymentGateway  │
                                                 │ PaymentRef      │
         │                                     │ CreatedAt       │
         │ N:M                                 └─────────────────┘
         │
┌─────────────────┐
│ AIServices      │
├─────────────────┤
│ Id (PK)         │
│ Name            │◄──────┐
│ TokenCost       │       │ N:1
│ IsActive        │       │
│ EndpointUrl     │       │
└─────────────────┘       │
         │                │
         │ 1:N            │
         │                │
┌─────────────────┐       │       ┌─────────────────┐
│ ServiceUsage    │       │       │ GeneratedContent│
├─────────────────┤       │       ├─────────────────┤
│ Id (PK)         │       │       │ Id (PK)         │
│ UserId (FK)     │       │       │ UsageId (FK)    │
│ ServiceId (FK)  │───────┘       │ FileUrl         │
│ TokensUsed      │               │ FileType        │
│ Cost            │               │ FileSize        │
│ Status          │               │ Metadata        │
│ CreatedAt       │               │ CreatedAt       │
└─────────────────┘               └─────────────────┘
```

#### Database Schema (SQL Server)

```sql
-- Users Table
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    EmailVerified BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    INDEX IX_Users_Email (Email)
);

-- UserProfiles Table
CREATE TABLE UserProfiles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Phone NVARCHAR(20),
    AvatarUrl NVARCHAR(500),
    Address NVARCHAR(500),
    City NVARCHAR(100),
    Country NVARCHAR(100),
    DateOfBirth DATE,
    CONSTRAINT FK_UserProfiles_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    UNIQUE (UserId)
);

-- TokenPlans Table
CREATE TABLE TokenPlans (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    TokenAmount INT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    IsActive BIT DEFAULT 1,
    Description NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- TokenBalances Table
CREATE TABLE TokenBalances (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Balance INT DEFAULT 0,
    ReservedBalance INT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT FK_TokenBalances_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    UNIQUE (UserId)
);

-- Transactions Table
CREATE TABLE Transactions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TransactionId NVARCHAR(100) NOT NULL UNIQUE,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Amount INT NOT NULL,
    Type NVARCHAR(50) NOT NULL, -- 'PURCHASE', 'USAGE', 'REFUND'
    Status NVARCHAR(50) NOT NULL, -- 'PENDING', 'COMPLETED', 'FAILED', 'REFUNDED'
    PaymentGateway NVARCHAR(50), -- 'BKASH', 'NAGAD', 'STRIPE'
    PaymentRef NVARCHAR(200),
    PlanId INT NULL,
    TokensBefore INT,
    TokensAfter INT,
    Description NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Transactions_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_Transactions_Plans FOREIGN KEY (PlanId) REFERENCES TokenPlans(Id),
    INDEX IX_Transactions_UserId (UserId),
    INDEX IX_Transactions_CreatedAt (CreatedAt)
);

-- AIServices Table
CREATE TABLE AIServices (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    TokenCost INT NOT NULL,
    Unit NVARCHAR(50), -- 'per_image', 'per_logo', 'per_minute', 'per_100_messages'
    EndpointUrl NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- ServiceUsage Table
CREATE TABLE ServiceUsage (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    ServiceId UNIQUEIDENTIFIER NOT NULL,
    TokensUsed INT NOT NULL,
    Cost DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL, -- 'PROCESSING', 'COMPLETED', 'FAILED'
    RequestData NVARCHAR(MAX),
    ResponseData NVARCHAR(MAX),
    ErrorMessage NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2,
    CONSTRAINT FK_ServiceUsage_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_ServiceUsage_AIServices FOREIGN KEY (ServiceId) REFERENCES AIServices(Id),
    INDEX IX_ServiceUsage_UserId_CreatedAt (UserId, CreatedAt)
);

-- GeneratedContent Table
CREATE TABLE GeneratedContent (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UsageId UNIQUEIDENTIFIER NOT NULL,
    FileUrl NVARCHAR(500) NOT NULL,
    FileType NVARCHAR(50), -- 'IMAGE', 'AUDIO', 'TEXT'
    FileSize BIGINT,
    Metadata NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT FK_GeneratedContent_ServiceUsage FOREIGN KEY (UsageId) REFERENCES ServiceUsage(Id),
    INDEX IX_GeneratedContent_UsageId (UsageId)
);

-- Notifications Table
CREATE TABLE Notifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Type NVARCHAR(50) NOT NULL, -- 'EMAIL', 'SMS', 'IN_APP', 'PUSH'
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    IsRead BIT DEFAULT 0,
    SentAt DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    INDEX IX_Notifications_UserId (UserId),
    INDEX IX_Notifications_IsRead (IsRead)
);

-- AuditLogs Table
CREATE TABLE AuditLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NULL,
    ActionType NVARCHAR(100) NOT NULL,
    EntityName NVARCHAR(100) NOT NULL,
    EntityId NVARCHAR(100),
    OldValues NVARCHAR(MAX),
    NewValues NVARCHAR(MAX),
    IpAddress NVARCHAR(50),
    UserAgent NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    INDEX IX_AuditLogs_CreatedAt (CreatedAt)
);
```

---

## Payment Gateway Integration

### bKash Integration

#### Prerequisites
1. **bKash Merchant Account**
   - Register as a merchant at bKash Merchant Portal
   - Get API credentials: App Key, App Secret, Username, Password
   - Obtain sandbox credentials for testing

2. **API Endpoints**
   - **Sandbox:** `https://tokenized.sandbox.bka.sh/v1.2.0-beta`
   - **Production:** `https://tokenized.pay.bka.sh/v1.2.0-beta`

#### Implementation Steps

##### Step 1: Configuration
```json
{
  "PaymentGateways": {
    "Bkash": {
      "BaseUrl": "https://tokenized.sandbox.bka.sh/v1.2.0-beta",
      "AppKey": "your_app_key",
      "AppSecret": "your_app_secret",
      "Username": "your_username",
      "Password": "your_password",
      "CallbackUrl": "https://yourdomain.com/api/payment/webhook/bkash",
      "IsSandbox": true
    }
  }
}
```

##### Step 2: Service Implementation
```csharp
public interface IBkashPaymentService
{
    Task<BkashGrantTokenResponse> GetGrantTokenAsync();
    Task<BkashCreatePaymentResponse> CreatePaymentAsync(BkashPaymentRequest request);
    Task<BkashExecutePaymentResponse> ExecutePaymentAsync(string paymentId);
    Task<BkashQueryPaymentResponse> QueryPaymentAsync(string paymentId);
}

public class BkashPaymentService : IBkashPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private string _grantToken;
    private DateTime _tokenExpiry;

    public BkashPaymentService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<BkashGrantTokenResponse> GetGrantTokenAsync()
    {
        if (_grantToken != null && _tokenExpiry > DateTime.UtcNow.AddMinutes(5))
        {
            return new BkashGrantTokenResponse { IdToken = _grantToken };
        }

        var baseUrl = _config["PaymentGateways:Bkash:BaseUrl"];
        var url = $"{baseUrl}/tokenized/checkout/token/grant";

        var headers = new Dictionary<string, string>
        {
            { "username", _config["PaymentGateways:Bkash:Username"] },
            { " "password", _config["PaymentGateways:Bkash:Password"] }
        };

        var response = await _httpClient.PostAsync(url, 
            new StringContent(JsonSerializer.Serialize(headers), Encoding.UTF8, "application/json"));

        var result = await response.Content.ReadFromJsonAsync<BkashGrantTokenResponse>();
        
        _grantToken = result.IdToken;
        _tokenExpiry = DateTime.UtcNow.AddHours(result.ExpiresIn / 3600);

        return result;
    }

    public async Task<BkashCreatePaymentResponse> CreatePaymentAsync(BkashPaymentRequest request)
    {
        var token = await GetGrantTokenAsync();
        var baseUrl = _config["PaymentGateways:Bkash:BaseUrl"];
        var url = $"{baseUrl}/tokenized/checkout/create";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.IdToken}");
        _httpClient.DefaultRequestHeaders.Add("X-APP-Key", _config["PaymentGateways:Bkash:AppKey"]);

        var response = await _httpClient.PostAsync(url,
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        return await response.Content.ReadFromJsonAsync<BkashCreatePaymentResponse>();
    }

    public async Task<BkashExecutePaymentResponse> ExecutePaymentAsync(string paymentId)
    {
        var token = await GetGrantTokenAsync();
        var baseUrl = _config["PaymentGateways:Bkash:BaseUrl"];
        var url = $"{baseUrl}/tokenized/checkout/execute";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.IdToken}");
        _httpClient.DefaultRequestHeaders.Add("X-APP-Key", _config["PaymentGateways:Bkash:AppKey"]);

        var request = new { paymentID = paymentId };
        var response = await _httpClient.PostAsync(url,
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        return await response.Content.ReadFromJsonAsync<BkashExecutePaymentResponse>();
    }

    public async Task<BkashQueryPaymentResponse> QueryPaymentAsync(string paymentId)
    {
        var token = await GetGrantTokenAsync();
        var baseUrl = _config["PaymentGateways:Bkash:BaseUrl"];
        var url = $"{baseUrl}/tokenized/checkout/payment/status";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.IdToken}");
        _httpClient.DefaultRequestHeaders.Add("X-APP-Key", _config["PaymentGateways:Bkash:AppKey"]);

        var request = new { paymentID = paymentId };
        var response = await _httpClient.PostAsync(url,
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        return await response.Content.ReadFromJsonAsync<BkashQueryPaymentResponse>();
    }
}
```

#### Payment Flow

1. **Initiate Payment**
```
User → Frontend → API → TokenService
              ↓
         PaymentService.CreatePaymentAsync()
              ↓
         bKash API (Create Payment)
              ↓
         Return bKash Checkout URL
              ↓
         Frontend redirects to bKash
```

2. **Complete Payment**
```
User completes payment on bKash
              ↓
         bKash redirects to Callback URL
              ↓
         PaymentService.ExecutePaymentAsync()
              ↓
         TokenService credit tokens
              ↓
         Notify user
```

### Nagad Integration

Similar approach to bKash with Nagad-specific API endpoints and authentication.

### Stripe Integration

For international card payments:

```csharp
public interface IStripePaymentService
{
    Task<Session> CreateCheckoutSessionAsync(decimal amount, string successUrl, string cancelUrl);
    Task<PaymentIntent> ConfirmPaymentAsync(string paymentIntentId);
}

public class StripePaymentService : IStripePaymentService
{
    private readonly IConfiguration _config;

    public StripePaymentService(IConfiguration config)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
        _config = config;
    }

    public async Task<Session> CreateCheckoutSessionAsync(decimal amount, string successUrl, string cancelUrl)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "bdt",
                        UnitAmount = (long)(amount * 100), // Convert to cents
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Token Purchase"
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            Metadata = new Dictionary<string, string>
            {
                { "purpose", "token_purchase" }
            }
        };

        var service = new SessionService();
        return await service.CreateAsync(options);
    }
}
```

---

## AI Services Integration

### Image Generation Service

#### Integration with OpenAI DALL-E

```csharp
public interface IImageGenerationService
{
    Task<GeneratedImageResponse> GenerateImageAsync(ImageGenerationRequest request);
}

public class OpenAiImageGenerationService : IImageGenerationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public OpenAiImageGenerationService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<GeneratedImageResponse> GenerateImageAsync(ImageGenerationRequest request)
    {
        var apiKey = _config["OpenAI:ApiKey"];
        var url = "https://api.openai.com/v1/images/generations";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var openAiRequest = new
        {
            model = "dall-e-3",
            prompt = request.Prompt,
            n = request.NumberOfImages,
            size = request.ImageSize,
            response_format = "url"
        };

        var response = await _httpClient.PostAsync(url,
            new StringContent(JsonSerializer.Serialize(openAiRequest), Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"OpenAI API error: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<OpenAiImageResponse>();
        
        return new GeneratedImageResponse
        {
            Images = result.Data.Select(d => d.Url).ToList(),
            RevisedPrompt = result.Data.FirstOrDefault()?.RevisedPrompt
        };
    }
}
```

### Logo Design Service

#### Integration with Custom AI Model or Midjourney API

```csharp
public interface ILogoDesignService
{
    Task<LogoDesignResponse> DesignLogoAsync(LogoDesignRequest request);
}

public class MidjourneyLogoDesignService : ILogoDesignService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public async Task<LogoDesignResponse> DesignLogoAsync(LogoDesignRequest request)
    {
        // Implement Midjourney API integration
        // Or use custom trained model
        throw new NotImplementedException();
    }
}
```

### Audio Generation Service

#### Integration with ElevenLabs API

```csharp
public interface IAudioGenerationService
{
    Task<GeneratedAudioResponse> GenerateAudioAsync(AudioGenerationRequest request);
}

public class ElevenLabsAudioService : IAudioGenerationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public async Task<GeneratedAudioResponse> GenerateAudioAsync(AudioGenerationRequest request)
    {
        var apiKey = _config["ElevenLabs:ApiKey"];
        var url = $"https://api.elevenlabs.io/v1/text-to-speech/{request.VoiceId}";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Accept", "audio/mpeg");
        _httpClient.DefaultRequestHeaders.Add("xi-api-key", apiKey);

        var requestBody = new
        {
            text = request.Text,
            model_id = "eleven_multilingual_v2",
            voice_settings = new
            {
                stability = request.Stability,
                similarity_boost = request.SimilarityBoost
            }
        };

        var response = await _httpClient.PostAsync(url,
            new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"ElevenLabs API error: {error}");
        }

        var audioBytes = await response.Content.ReadAsByteArrayAsync();
        var audioUrl = await StoreAudioAsync(audioBytes);

        return new GeneratedAudioResponse
        {
            AudioUrl = audioUrl,
            Duration = CalculateAudioDuration(audioBytes.Length)
        };
    }

    private async Task<string> StoreAudioAsync(byte[] audioBytes)
    {
        // Store to Azure Blob Storage or AWS S3
        // Return the stored URL
        throw new NotImplementedException();
    }

    private double CalculateAudioDuration(int fileSize)
    {
        // Approximate calculation for MP3
        return fileSize / 32000.0; // 32 kbps
    }
}
```

### Chatbot Service

#### Integration with OpenAI GPT-4

```csharp
public interface IChatbotService
{
    Task<ChatResponse> SendMessageAsync(ChatRequest request);
}

public class OpenAiChatService : IChatbotService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly IMemoryCache _cache;

    public async Task<ChatResponse> SendMessageAsync(ChatRequest request)
    {
        var apiKey = _config["OpenAI:ApiKey"];
        var url = "https://api.openai.com/v1/chat/completions";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var messages = new List<object>
        {
            new { role = "system", content = request.SystemPrompt ?? "You are a helpful assistant." }
        };

        // Load conversation history if provided
        if (request.ConversationId.HasValue)
        {
            var history = await GetConversationHistoryAsync(request.ConversationId.Value);
            messages.AddRange(history);
        }

        messages.Add(new { role = "user", content = request.Message });

        var requestBody = new
        {
            model = "gpt-4",
            messages = messages,
            temperature = request.Temperature ?? 0.7,
            max_tokens = request.MaxTokens ?? 1000
        };

        var response = await _httpClient.PostAsync(url,
            new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"OpenAI API error: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<OpenAiChatResponse>();
        
        // Save conversation
        if (request.ConversationId.HasValue)
        {
            await SaveConversationAsync(request.ConversationId.Value, request.Message, 
                result.Choices[0].Message.Content);
        }

        return new ChatResponse
        {
            Message = result.Choices[0].Message.Content,
            TokensUsed = result.Usage.TotalTokens,
            ConversationId = request.ConversationId ?? Guid.NewGuid()
        };
    }
}
```

---

## Technology Stack

### Backend Technologies

#### Core Framework
- **ASP.NET Core 8.0/9.0**
  - Cross-platform support
  - High performance
  - Built-in dependency injection
  - Excellent middleware support

#### API Documentation
- **Swashbuckle (Swagger)**
  - Auto-generate API documentation
  - Interactive API testing UI
  - Versioning support

#### Authentication & Authorization
- **ASP.NET Core Identity**
  - User management
  - Role-based authorization
  - Password hashing
  - Two-factor authentication

#### Database ORM
- **Entity Framework Core 8.0**
  - Code-first migrations
  - LINQ queries
  - Change tracking
  - Database providers for SQL Server/PostgreSQL

#### Caching
- **Redis Cache**
  - Distributed caching
  - Session state management
  - Token balance caching
  - Real-time rate limiting

#### Message Queue
- **RabbitMQ / MassTransit**
  - Asynchronous processing
  - Payment notifications
  - AI service job queue
  - Email queue

#### Logging
- **Serilog**
  - Structured logging
  - Multiple sinks (file, database, Elasticsearch)
  - Log correlation
  - Request/response logging

#### API Gateway
- **Ocelot / YARP**
  - Request routing
  - Load balancing
  - Rate limiting
  - API composition

### Frontend Technologies

#### Option 1: Blazor Server/WASM
- **Blazor Server**
  - Server-side rendering
  - Real-time via SignalR
  - Fast initial load
  - C# on both sides

- **Blazor WASM**
  - Client-side rendering
  - PWA support
  - Offline capability
  - Smaller server footprint

#### Option 2: React
- **Next.js 14**
  - Server-side rendering
  - Static site generation
  - API routes
  - Image optimization

- **UI Components**
  - Material-UI / Ant Design
  - TailwindCSS
  - React Query for data fetching

### Database Technologies

#### Primary Database
- **SQL Server 2022** (Azure SQL)
  - Enterprise-grade
  - High availability
  - Built-in backup
  - Performance tuning

**OR**

- **PostgreSQL 15**
  - Open-source
  - Advanced features
  - JSON support
  - Cost-effective

#### Caching Layer
- **Redis 7.x**
  - In-memory data store
  - Pub/Sub support
  - Geospatial support
  - Clustering support

### Storage Solutions

#### File Storage
- **Azure Blob Storage**
  - Scalable object storage
  - CDN integration
  - Lifecycle policies
  - Cost-effective tiers

**OR**

- **AWS S3**
  - Industry standard
  - Multi-region replication
  - Version control
  - Event notifications

### DevOps & Deployment

#### Container Orchestration
- **Docker**
  - Containerization
  - Consistent environments
  - Easy deployment

- **Kubernetes** (Optional for scaling)
  - Auto-scaling
  - Self-healing
  - Load balancing
  - Rollback capabilities

#### CI/CD
- **GitHub Actions**
  - Automated testing
  - Automated deployment
  - Environment promotion
  - Rollback strategies

#### Monitoring
- **Application Insights** (Azure)
  - Application performance monitoring
  - Error tracking
  - User analytics
  - Custom metrics

**OR**

- **Prometheus + Grafana**
  - Metrics collection
  - Visualization dashboards
  - Alert management
  - Open-source

---

## Implementation Plan

### Phase 1: Project Setup & Foundation (Weeks 1-2)

#### Week 1: Environment Setup
- [ ] Set up development environment
  - Install .NET 8 SDK
  - Install Visual Studio 2022 / JetBrains Rider
  - Install Docker Desktop
  - Set up Git repository
  - Configure branch protection rules

- [ ] Create solution structure
  - Initialize ASP.NET Core Web API project
  - Set up solution folders and layers
  - Configure project references
  - Set up coding standards (.editorconfig)

- [ ] Configure database
  - Install SQL Server / PostgreSQL
  - Set up database connection strings
  - Configure EF Core migrations
  - Create initial migration

- [ ] Set up Redis
  - Install Redis locally
  - Configure Redis connection
  - Test caching functionality

#### Week 2: Core Infrastructure
- [ ] Implement logging
  - Configure Serilog
  - Set up log sinks (file, console)
  - Implement request/response logging middleware
  - Configure log correlation

- [ ] Implement error handling
  - Create global exception handler
  - Implement error response DTOs
  - Set up error logging
  - Create custom exceptions

- [ ] Implement API documentation
  - Configure Swagger/OpenAPI
  - Add XML comments
  - Set up authentication in Swagger
  - Configure API versioning

- [ ] Implement health checks
  - Add health check endpoints
  - Configure database health check
  - Configure Redis health check
  - Configure external service health checks

**Acceptance Criteria:**
- [x] Solution builds successfully
- [x] All health check endpoints return healthy
- [x] Swagger UI accessible at /swagger
- [x] Logging writes to configured sinks
- [x] Database migrations run successfully
- [x] Redis connection established

---

### Phase 2: User Management (Weeks 3-4)

#### Week 3: Authentication
- [ ] Implement user registration
  - Create registration API endpoint
  - Implement email validation
  - Password strength validation
  - Send verification email

- [ ] Implement user login
  - Create login API endpoint
  - Generate JWT tokens
  - Implement refresh token mechanism
  - Add login rate limiting

- [ ] Implement email verification
  - Create verification endpoint
  - Generate verification tokens
  - Send verification emails
  - Handle token expiration

- [ ] Implement password management
  - Forgot password endpoint
  - Password reset flow
  - Change password endpoint
  - Password history validation

#### Week 4: User Profile & Authorization
- [ ] Implement user profiles
  - Create profile API endpoints
  - Implement profile CRUD operations
  - Add avatar upload
  - Profile completion tracking

- [ ] Implement role-based authorization
  - Define user roles (User, Admin)
  - Implement authorization policies
  - Add role assignment APIs
  - Test authorization on endpoints

- [ ] Implement two-factor authentication (optional)
  - Configure TOTP provider
  - Add 2FA setup endpoint
  - Implement 2FA verification
  - Add recovery codes

**Acceptance Criteria:**
- [x] Users can register with email
- [x] Users receive verification emails
- [x] Users can login with valid credentials
- [x] JWT tokens are generated and validated
- [x] Password reset flow works end-to-end
- [x] User profiles can be created and updated
- [x] Role-based authorization protects endpoints
- [x] All endpoints have proper error handling

---

### Phase 3: Token Management System (Weeks 5-6)

#### Week 5: Token Core
- [ ] Implement token balance tracking
  - Create TokenBalance entity
  - Implement balance queries
  - Add balance caching with Redis
  - Implement balance locking for transactions

- [ ] Implement token plans
  - Create TokenPlan entity
  - Define pricing tiers
  - Add plan management APIs (admin)
  - Implement plan activation/deactivation

- [ ] Implement transaction tracking
  - Create Transaction entity
  - Implement transaction recording
  - Add transaction history APIs
  - Implement transaction pagination

#### Week 6: Token Operations
- [ ] Implement token purchase
  - Create purchase API endpoint
  - Validate payment before credit
  - Implement token credit logic
  - Add purchase notifications

- [ ] Implement token deduction
  - Create deduction service
  - Validate sufficient balance
  - Implement atomic deduction
  - Handle insufficient funds

- [ ] Implement refund logic
  - Create refund API (admin)
  - Implement token return
  - Add refund transaction
  - Send refund notifications

**Acceptance Criteria:**
- [x] Token balances are accurate
- [x] Token plans can be defined and retrieved
- [x] Token purchase credits balance correctly
- [x] Token deduction is atomic and consistent
- [x] Transaction history is complete
- [x] Balance caching improves performance
- [x] Refunds work correctly
- [x] Concurrent transactions don't cause data corruption

---

### Phase 4: Payment Gateway Integration (Weeks 7-9)

#### Week 7: bKash Integration
- [ ] Implement bKash authentication
  - Create grant token service
  - Implement token caching
  - Handle token refresh
  - Add error handling

- [ ] Implement payment creation
  - Create payment initiation API
  - Generate bKash checkout URL
  - Implement webhook registration
  - Add payment status tracking

- [ ] Implement payment execution
  - Create payment execution service
  - Handle bKash callbacks
  - Validate payment authenticity
  - Update payment status

- [ ] Implement payment query
  - Add payment status verification
  - Implement periodic reconciliation
  - Handle failed payments
  - Add payment notifications

#### Week 8: Nagad Integration
- [ ] Implement Nagad authentication
- [ ] Implement payment creation
- [ ] Implement payment execution
- [ ] Implement payment query

#### Week 9: Stripe Integration
- [ ] Implement Stripe checkout
  - Create checkout session API
  - Configure success/failure URLs
  - Handle Stripe webhooks
  - Add payment verification

- [ ] Implement payment reconciliation
  - Daily payment reconciliation job
  - Transaction matching
  - Discrepancy reporting
  - Automatic refunds for failures

**Acceptance Criteria:**
- [x] bKash payment flow works end-to-end
- [x] Nagad payment flow works end-to-end
- [x] Stripe payment flow works end-to-end
- [x] Payment callbacks are authenticated
- [x] Tokens are credited only after successful payment
- [x] Failed payments don't credit tokens
- [x] Payment reconciliation works
- [x] All payment gateways are secure

---

### Phase 5: AI Services Integration (Weeks 10-14)

#### Week 10: Image Generation Service
- [ ] Integrate OpenAI DALL-E
  - Implement image generation API
  - Handle DALL-E responses
  - Store generated images
  - Implement usage tracking

- [ ] Add cost calculation
  - Calculate token cost before generation
  - Validate user balance
  - Deduct tokens after generation
  - Handle failed generations

- [ ] Implement job queue
  - Queue image generation requests
  - Process jobs asynchronously
  - Update job status
  - Notify users on completion

#### Week 11: Logo Design Service
- [ ] Integrate logo design API
  - Implement logo generation
  - Handle custom parameters
  - Store generated logos
  - Implement revision workflow

- [ ] Add cost calculation
- [ ] Implement job queue

#### Week 12: Audio Generation Service
- [ ] Integrate ElevenLabs API
  - Implement text-to-speech
  - Handle voice selection
  - Store generated audio
  - Implement audio preview

- [ ] Add cost calculation
- [ ] Implement job queue

#### Week 13: Chatbot Service
- [ ] Integrate OpenAI GPT-4
  - Implement chat API
  - Manage conversation history
  - Handle context window
  - Implement streaming responses

- [ ] Add cost calculation
  - Calculate token cost per message
  - Track conversation tokens
  - Implement rate limiting

#### Week 14: Service Orchestration
- [ ] Implement service dispatcher
  - Route requests to correct service
  - Handle service-specific logic
  - Implement unified response format
  - Add service health checks

- [ ] Implement usage analytics
  - Track service usage
  - Generate usage reports
  - Identify popular services
  - Monitor service performance

**Acceptance Criteria:**
- [x] Image generation works with DALL-E
- [x] Logo generation produces quality results
- [x] Audio generation works with ElevenLabs
- [x] Chatbot conversations are coherent
- [x] All services deduct correct token amounts
- [x] Failed services don't deduct tokens
- [x] Service responses are stored
- [x] Users can retrieve their generated content
- [x] Usage analytics are accurate

---

### Phase 6: Notification System (Weeks 15-16)

#### Week 15: Email Notifications
- [ ] Configure email service
  - Set up SMTP server
  - Configure email templates
  - Implement email queue
  - Add email tracking

- [ ] Implement notification types
  - Registration confirmation
  - Email verification
  - Password reset
  - Token purchase confirmation
  - Service completion notifications
  - Low balance warnings

#### Week 16: Other Notifications
- [ ] Implement SMS notifications
  - Integrate SMS gateway
  - Add SMS templates
  - Implement SMS queue
  - Handle SMS failures

- [ ] Implement in-app notifications
  - Create notification entity
  - Implement notification API
  - Add real-time notifications (SignalR)
  - Implement notification preferences

**Acceptance Criteria:**
- [x] Emails are sent for all key events
- [x] Email templates are professional
- [x] SMS notifications work for critical alerts
- [x] In-app notifications are real-time
- [x] Users can manage notification preferences
- [x] Notification queue handles high volume
- [x] Failed notifications are logged

---

### Phase 7: Admin Dashboard (Weeks 17-18)

#### Week 17: Admin Features
- [ ] Implement user management
  - List all users
  - View user details
  - Manage user status
  - View user activity

- [ ] Implement revenue tracking
  - Daily revenue reports
  - Revenue by payment gateway
  - Revenue by token plan
  - Revenue trends

#### Week 18: Analytics & Monitoring
- [ ] Implement usage analytics
  - Service usage statistics
  - Active user counts
  - Token consumption trends
  - Peak usage times

- [ ] Implement system monitoring
  - Server health metrics
  - Database performance
  - API response times
  - Error rates

**Acceptance Criteria:**
- [x] Admins can manage users
- [x] Revenue reports are accurate
- [x] Usage analytics provide insights
- [x] System health is monitored
- [x] Alerts are configured for critical issues
- [x] Admin dashboard is responsive

---

### Phase 8: Frontend Development (Weeks 19-22)

#### Week 19-20: Web Frontend
- [ ] Set up frontend project
  - Initialize Blazor/React project
  - Configure routing
  - Set up state management
  - Configure API client

- [ ] Implement authentication UI
  - Login page
  - Registration page
  - Email verification
  - Password reset

- [ ] Implement dashboard
  - User profile page
  - Token balance display
  - Purchase tokens page
  - Transaction history

#### Week 21-22: Service Interfaces
- [ ] Implement AI service UIs
  - Image generator interface
  - Logo designer interface
  - Audio generator interface
  - Chatbot interface

- [ ] Implement gallery/history
  - Generated content gallery
  - Download functionality
  - Share functionality
  - Delete functionality

**Acceptance Criteria:**
- [x] Frontend is responsive and accessible
- [x] All authentication flows work
- [x] Token purchase flow is seamless
- [x] AI service interfaces are intuitive
- [x] Generated content is displayed properly
- [x] Error messages are user-friendly
- [x] Loading states are visible

---

### Phase 9: Testing & Quality Assurance (Weeks 23-24)

#### Week 23: Testing
- [ ] Unit testing
  - Test business logic
  - Test data access layer
  - Test service layer
  - Aim for 80%+ code coverage

- [ ] Integration testing
  - Test API endpoints
  - Test database operations
  - Test payment gateway integration
  - Test AI service integration

#### Week 24: Quality Assurance
- [ ] Performance testing
  - Load test API endpoints
  - Test concurrent users
  - Optimize slow queries
  - Implement caching strategies

- [ ] Security testing
  - Penetration testing
  - SQL injection testing
  - XSS testing
  - CSRF testing

- [ ] UAT (User Acceptance Testing)
  - Test with real users
  - Gather feedback
  - Fix identified issues
  - Finalize features

**Acceptance Criteria:**
- [x] All unit tests pass
- [x] All integration tests pass
- [x] API handles expected load
- [x] No security vulnerabilities
- [x] UAT feedback is positive
- [x] All critical bugs are fixed

---

### Phase 10: Deployment & Launch (Weeks 25-26)

#### Week 25: Deployment Setup
- [ ] Configure cloud infrastructure
  - Set up Azure/AWS account
  - Configure virtual networks
  - Set up databases
  - Configure Redis cache

- [ ] Configure CI/CD
  - Set up GitHub Actions
  - Configure build pipeline
  - Configure deployment pipeline
  - Set up environment variables

- [ ] Configure monitoring
  - Set up Application Insights
  - Configure alerts
  - Set up log aggregation
  - Configure dashboards

#### Week 26: Launch
- [ ] Perform production deployment
  - Deploy to production environment
  - Run database migrations
  - Configure DNS
  - Set up SSL certificates

- [ ] Final checks
  - Smoke test all features
  - Verify monitoring
  - Test backup/restore
  - Document runbook

- [ ] Launch
  - Make platform live
  - Monitor for issues
  - Be ready to rollback
  - Celebrate! 🎉

**Acceptance Criteria:**
- [x] Production deployment succeeds
- [x] All features work in production
- [x] Monitoring is active
- [x] Backups are configured
- [x] SSL is properly configured
- [x] Documentation is complete

---

## Security Considerations

### Authentication & Authorization

#### 1. JWT Token Security
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),
            ClockSkew = TimeSpan.Zero
        };
    });
```

#### 2. Password Policy
- Minimum 12 characters
- Require uppercase, lowercase, numbers, special characters
- Prevent common passwords
- Hash with ASP.NET Core Identity (PBKDF2)
- Implement password expiration

#### 3. Rate Limiting
```csharp
services.AddRateLimiter(options =>
{
    options.AddPolicy("LoginPolicy", context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(5),
                SegmentsPerWindow = 2
            }));

    options.AddPolicy("ApiPolicy", context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 10
            }));
});
```

### Data Protection

#### 1. Encryption
- Encrypt sensitive data at rest
- Use TLS 1.3 for data in transit
- Encrypt API keys and secrets
- Use Azure Key Vault for secret management

#### 2. Input Validation
```csharp
public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(12)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$");

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[0-9]{10,15}$");
    }
}
```

#### 3. SQL Injection Prevention
- Always use parameterized queries
- Use EF Core LINQ queries
- Never concatenate user input

```csharp
// BAD
var query = $"SELECT * FROM Users WHERE Email = '{email}'";

// GOOD
var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
```

### Payment Security

#### 1. Webhook Authentication
```csharp
[HttpPost("webhook/bkash")]
public async Task<IActionResult> BkashWebhook([FromBody] BkashWebhookPayload payload, 
    [FromHeader(Name = "X-App-Key")] string appKey)
{
    // Verify app key
    if (appKey != _configuration["PaymentGateways:Bkash:AppKey"])
    {
        return Unauthorized();
    }

    // Verify signature
    var signature = Request.Headers["X-Signature"];
    var isValidSignature = VerifySignature(payload, signature);
    if (!isValidSignature)
    {
        return Unauthorized();
    }

    // Process webhook
    await _paymentService.ProcessWebhookAsync(payload);
    return Ok();
}
```

#### 2. PCI DSS Compliance
- Never store full credit card numbers
- Use tokenization
- Ensure payment gateways are PCI compliant
- Regular security audits

### API Security

#### 1. CORS Configuration
```csharp
services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", builder =>
    {
        builder.WithOrigins("https://yourdomain.com")
               .WithMethods("GET", "POST", "PUT", "DELETE")
               .AllowCredentials();
    });
});
```

#### 2. HTTPS Enforcement
```csharp
services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

app.UseHsts();
app.UseHttpsRedirection();
```

#### 3. Security Headers
```csharp
app.UseSecurityHeaders(new HeaderPolicyCollection()
    .AddFrameOptionsSameOrigin()
    .AddXssProtectionBlock()
    .AddContentTypeOptionsNoSniff()
    .AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365)
    .AddReferrerPolicyStrictOriginWhenCrossOrigin()
    .AddContentSecurityPolicy(builder =>
    {
        builder.AddDefaultSrc().Self();
        builder.AddImgSrc().Self().Data();
        builder.AddScriptSrc().Self();
    })
);
```

---

## Deployment Strategy

### Development Environment

#### Local Development
```yaml
# docker-compose.dev.yml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: YourStrong@Password
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: admin
      RABBITMQ_DEFAULT_PASS: admin

volumes:
  sqlserver_data:
  redis_data:
```

### Staging Environment

#### Azure Resources
- **App Service Plan** (Standard S1)
- **Azure SQL Database** (Standard S0)
- **Redis Cache** (Standard C0)
- **Blob Storage** (Standard LRS)
- **Application Insights**

#### Deployment Script
```bash
# deploy-staging.sh
#!/bin/bash

az webapp deployment source sync-zip \
  --resource-group ai-services-staging \
  --name ai-services-api-staging \
  --src ./artifacts/AI.Services.API.zip

az webapp restart \
  --resource-group ai-services-staging \
  --name ai-services-api-staging

echo "Deployment to staging completed!"
```

### Production Environment

#### Azure Resources
- **App Service Plan** (Premium P1v2) - Auto-scale
- **Azure SQL Database** (Premium P2) - Geo-replication
- **Redis Cache** (Premium P1) - Cluster
- **Blob Storage** (Premium LRS) - CDN enabled
- **Application Insights** - Alert rules configured
- **Key Vault** - Secret management

#### High Availability Setup
- Multiple availability zones
- Database failover groups
- Redis cluster with replicas
- CDN for static assets
- Load balancer with health checks

#### Deployment Pipeline (GitHub Actions)
```yaml
# .github/workflows/deploy-production.yml
name: Deploy to Production

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Build
      run: dotnet build --configuration Release
    
    - name: Run Tests
      run: dotnet test --configuration Release --no-build --verbosity normal
    
    - name: Publish
      run: dotnet publish --configuration Release --output ./publish
    
    - name: Zip Artifact
      run: zip -r ./artifacts/AI.Services.API.zip ./publish/*
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'ai-services-api-production'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./artifacts/AI.Services.API.zip
    
    - name: Run Database Migrations
      run: |
        dotnet ef database update --connection "${{ secrets.AZURE_SQL_CONNECTION_STRING }}"
    
    - name: Health Check
      run: |
        curl -f https://api.yourdomain.com/health || exit 1
```

### Rollback Strategy

#### Blue-Green Deployment
1. Deploy new version to green environment
2. Run smoke tests on green
3. Switch traffic from blue to green
4. Monitor for issues
5. Rollback if needed (switch traffic back)

#### Database Rollback
```bash
# Rollback last migration
dotnet ef database update previous-migration --connection "<connection-string>"

# Or rollback to specific version
dotnet ef database update 20240301000000_InitialCreate --connection "<connection-string>"
```

---

## Monitoring & Maintenance

### Application Monitoring

#### Application Insights Setup
```csharp
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

builder.Services.AddApplicationInsightsKubernetesEnricher();
```

#### Key Metrics to Monitor
- **Performance**
  - API response times (p50, p95, p99)
  - Database query performance
  - Cache hit rates
  - Memory usage

- **Business**
  - Active users
  - Token purchases
  - AI service usage
  - Revenue

- **Errors**
  - HTTP 5xx errors
  - Unhandled exceptions
  - Payment failures
  - AI service failures

#### Alert Configuration
```json
{
  "alerts": [
    {
      "name": "High Error Rate",
      "condition": "error_rate > 5%",
      "threshold": 5,
      "window": "5m",
      "actions": ["email", "slack"]
    },
    {
      "name": "Slow API Response",
      "condition": "p95_response_time > 2000ms",
      "threshold": 2000,
      "window": "5m",
      "actions": ["email"]
    },
    {
      "name": "Database Connection Pool Exhausted",
      "condition": "db_connections > 90%",
      "threshold": 90,
      "window": "1m",
      "actions": ["email", "slack", "pagerduty"]
    }
  ]
}
```

### Log Management

#### Serilog Configuration
```csharp
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "AI.Services.API")
    .WriteTo.Console()
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.ApplicationInsights(
        services.GetRequiredService<TelemetryConfiguration>(),
        TelemetryConverter.Traces));
```

### Backup Strategy

#### Database Backup
```sql
-- Full backup daily
BACKUP DATABASE [AI.Services] 
TO DISK = N'https://storage.blob.core.windows.net/backups/AI.Services_full.bak'
WITH COMPRESSION, ENCRYPTION;

-- Differential backup every 4 hours
BACKUP DATABASE [AI.Services] 
TO DISK = N'https://storage.blob.core.windows.net/backups/AI.Services_diff.bak'
WITH DIFFERENTIAL, COMPRESSION, ENCRYPTION;

-- Transaction log backup every 15 minutes
BACKUP LOG [AI.Services] 
TO DISK = N'https://storage.blob.core.windows.net/backups/AI.Services_log.trn'
WITH COMPRESSION, ENCRYPTION;
```

#### Blob Storage Backup
- Enable versioning
- Enable point-in-time restore
- Cross-region replication
- Lifecycle policies

### Maintenance Tasks

#### Scheduled Tasks
```csharp
// Delete old generated content
public class ContentCleanupJob
{
    [FunctionName("ContentCleanupJob")]
    public async Task Run(
        [TimerTrigger("0 0 2 * * *")] TimerInfo myTimer, // Runs daily at 2 AM
        ILogger logger)
    {
        var oldContent = await context.GeneratedContent
            .Where(c => c.CreatedAt < DateTime.UtcNow.AddDays(-30))
            .ToListAsync();

        foreach (var content in oldContent)
        {
            await blobStorage.DeleteAsync(content.FileUrl);
            context.GeneratedContent.Remove(content);
        }

        await context.SaveChangesAsync();
        logger.LogInformation($"Deleted {oldContent.Count} old content files");
    }
}
```

---

## Cost Estimation

### Development Costs (Initial)

#### Development Team
- **Senior Backend Developer:** $8,000 - $12,000/month × 6 months = $48,000 - $72,000
- **Senior Frontend Developer:** $7,000 - $10,000/month × 4 months = $28,000 - $40,000
- **DevOps Engineer:** $6,000 - $9,000/month × 2 months = $12,000 - $18,000
- **UI/UX Designer:** $3,000 - $5,000/project = $3,000 - $5,000
- **QA Engineer:** $4,000 - $6,000/month × 2 months = $8,000 - $12,000

**Total Development Cost:** $99,000 - $147,000

#### Software & Tools (Annual)
- Visual Studio Enterprise: $2,999
- JetBrains Rider: $199
- Azure/AWS credits: Included with commitment
- Domain & SSL: $100
- GitHub Copilot: $120
- **Total:** ~$3,500

### Infrastructure Costs (Monthly)

#### Production Environment (Azure)
- **App Service (Premium P1v2 - 2 instances):** $416
- **Azure SQL (Premium P2):** $1,250
- **Redis Cache (Premium P1):** $518
- **Blob Storage (10 TB):** $200
- **Application Insights:** $172
- **Azure CDN:** $100
- **Bandwidth (10 TB):** $800
- **Azure Key Vault:** $1
- **Load Balancer:** $18

**Total Production Infrastructure:** ~$3,475/month

#### Staging Environment (50% of production)
**Total Staging Infrastructure:** ~$1,750/month

**Total Monthly Infrastructure:** ~$5,225/month

### AI Service Costs

#### OpenAI API
- **GPT-4:** $0.03/1K input tokens, $0.06/1K output tokens
- **DALL-E 3:** $0.04/image (1024×1024)
- **Estimated monthly (1M users):** $2,000 - $10,000

#### ElevenLabs API
- **Text-to-Speech:** $0.30/1,000 characters
- **Estimated monthly:** $500 - $2,000

#### Midjourney (Logo)
- **Subscription:** $30/month

**Total AI Service Costs:** ~$2,500 - $12,000/month

### Payment Gateway Costs

#### bKash
- Setup fee: ৳5,000 (~$45)
- Transaction fee: 1.5%

#### Nagad
- Setup fee: ৳3,000 (~$27)
- Transaction fee: 1.5%

#### Stripe
- Transaction fee: 2.9% + $0.30

**Estimated monthly transaction fees:** 2% of revenue

### Total Monthly Operational Cost

| Category | Cost Range |
|----------|-----------|
| Infrastructure | $5,225 |
| AI Services | $2,500 - $12,000 |
| Payment Gateway Fees | ~2% of revenue |
| **Total** | $7,725 - $17,225 + 2% of revenue |

### Revenue Projections

#### Conservative Scenario
- 1,000 users
- Average 60 tokens/user/month
- Average revenue: ৳50/60 tokens = $0.46 USD
- Monthly revenue: 1,000 × $0.46 = $460/month
- **Break-even:** ~17-37 months

#### Moderate Scenario
- 5,000 users
- Average 120 tokens/user/month
- Average revenue: ৳90/120 tokens = $0.83 USD
- Monthly revenue: 5,000 × $0.83 = $4,150/month
- **Break-even:** ~2-4 months

#### Optimistic Scenario
- 20,000 users
- Average 200 tokens/user/month
- Average revenue: $0.92 USD
- Monthly revenue: 20,000 × $0.92 = $18,400/month
- **Profitable from Month 1**

---

## Conclusion

This comprehensive architecture and project plan provides a solid foundation for building a professional, scalable, and secure AI services platform with token-based billing. The plan follows best practices in software engineering, security, and cloud architecture.

### Key Takeaways:

1. **Microservices Architecture** - Ensures scalability and maintainability
2. **Token-Based System** - Flexible pricing model for users
3. **Multiple Payment Gateways** - Accessibility for all users
4. **Professional Security** - Protects user data and payments
5. **Comprehensive Monitoring** - Ensures system reliability
6. **Scalable Infrastructure** - Grows with user base
7. **Clear Implementation Plan** - 26-week development timeline

### Next Steps:

1. Review and refine this plan
2. Secure funding/resources
3. Assemble development team
4. Set up development environment
5. Begin Phase 1 implementation

---

## Appendix

### A. API Documentation Structure

```
/api/v1/
  ├── auth/
  │   ├── register
  │   ├── login
  │   ├── refresh-token
  │   ├── forgot-password
  │   └── verify-email
  ├── users/
  │   ├── profile
  │   ├── {userId}
  │   └── avatar
  ├── tokens/
  │   ├── balance
  │   ├── plans
  │   ├── purchase
  │   └── transactions
  ├── payments/
  │   ├── initiate
  │   ├── status/{transactionId}
  │   ├── webhook/bkash
  │   ├── webhook/nagad
  │   └── webhook/stripe
  ├── ai/
  │   ├── image-generate
  │   ├── logo-design
  │   ├── audio-generate
  │   ├── chatbot
  │   ├── jobs/{jobId}
  │   └── history
  ├── notifications/
  │   ├── /
  │   ├── mark-read/{id}
  │   └── preferences
  └── admin/
      ├── users
      ├── revenue
      ├── analytics
      └── system-health
```

### B. Database Migration Scripts

#### Initial Migration
```bash
# Create initial migration
dotnet ef migrations add InitialCreate --context ApplicationDbContext

# Apply migration to database
dotnet ef database update --context ApplicationDbContext
```

#### Seeding Data
```csharp
public class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed Token Plans
        if (!context.TokenPlans.Any())
        {
            var plans = new List<TokenPlan>
            {
                new TokenPlan
                {
                    Name = "Starter Pack",
                    TokenAmount = 60,
                    Price = 50m,
                    IsActive = true,
                    Description = "Perfect for trying out our AI services"
                },
                new TokenPlan
                {
                    Name = "Value Pack",
                    TokenAmount = 120,
                    Price = 90m,
                    IsActive = true,
                    Description = "Save 25% with our popular plan"
                },
                new TokenPlan
                {
                    Name = "Pro Pack",
                    TokenAmount = 300,
                    Price = 200m,
                    IsActive = true,
                    Description = "Best value for regular users - 33% savings"
                },
                new TokenPlan
                {
                    Name = "Enterprise Pack",
                    TokenAmount = 1000,
                    Price = 600m,
                    IsActive = true,
                    Description = "Maximum savings for power users - 40% off"
                }
            };

            await context.TokenPlans.AddRangeAsync(plans);
            await context.SaveChangesAsync();
        }

        // Seed AI Services
        if (!context.AIServices.Any())
        {
            var services = new List<AIService>
            {
                new AIService
                {
                    Name = "Image Generator",
                    Description = "Generate stunning images with AI",
                    TokenCost = 1,
                    Unit = "per_5_images",
                    IsActive = true
                },
                new AIService
                {
                    Name = "Logo Designer",
                    Description = "Create professional logos",
                    TokenCost = 3,
                    Unit = "per_logo",
                    IsActive = true
                },
                new AIService
                {
                    Name = "Audio Generator",
                    Description = "Convert text to natural speech",
                    TokenCost = 2,
                    Unit = "per_minute",
                    IsActive = true
                },
                new AIService
                {
                    Name = "Chatbot",
                    Description = "AI-powered conversations",
                    TokenCost = 1,
                    Unit = "per_100_messages",
                    IsActive = true
                }
            };

            await context.AIServices.AddRangeAsync(services);
            await context.SaveChangesAsync();
        }
    }
}
```

### C. Sample Configuration Files

#### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=AI.Services;User Id=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=true",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Issuer": "https://api.yourdomain.com",
    "Audience": "https://yourdomain.com",
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "ExpiryInMinutes": 60
  },
  "PaymentGateways": {
    "Bkash": {
      "BaseUrl": "https://tokenized.sandbox.bka.sh/v1.2.0-beta",
      "AppKey": "your_bkash_app_key",
      "AppSecret": "your_bkash_app_secret",
      "Username": "your_bkash_username",
      "Password": "your_bkash_password",
      "CallbackUrl": "https://yourdomain.com/api/payment/webhook/bkash",
      "IsSandbox": true
    },
    "Nagad": {
      "BaseUrl": "https://sandbox.mynagad.com",
      "MerchantId": "your_nagad_merchant_id",
      "PrivateKey": "your_nagad_private_key",
      "PublicKey": "nagad_public_key",
      "CallbackUrl": "https://yourdomain.com/api/payment/webhook/nagad",
      "IsSandbox": true
    },
    "Stripe": {
      "SecretKey": "sk_test_your_stripe_secret_key",
      "PublishableKey": "pk_test_your_stripe_publishable_key",
      "WebhookSecret": "whsec_your_webhook_secret",
      "IsSandbox": true
    }
  },
  "OpenAI": {
    "ApiKey": "sk-your-openai-api-key",
    "Organization": "org-your-organization-id"
  },
  "ElevenLabs": {
    "ApiKey": "your_elevenlabs_api_key"
  },
  "BlobStorage": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=youraccount;AccountKey=yourkey;EndpointSuffix=core.windows.net",
    "ContainerName": "generated-content"
  },
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=your-instrumentation-key;IngestionEndpoint=https://your-region.applicationinsights.azure.com/"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### D. Sample Docker Compose for Production

```yaml
# docker-compose.prod.yml
version: '3.8'

services:
  api:
    image: ai-services-api:latest
    ports:
      - "80:80"
      - "443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${DB_CONNECTION_STRING}
      - ConnectionStrings__Redis=${REDIS_CONNECTION_STRING}
      - Jwt__SecretKey=${JWT_SECRET_KEY}
    depends_on:
      - db
      - redis
    restart: unless-stopped
    networks:
      - ai-services-network

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: ${DB_PASSWORD}
    ports:
      - "1433:1433"
    volumes:
      - db_data:/var/opt/mssql
    restart: unless-stopped
    networks:
      - ai-services-network

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    restart: unless-stopped
    networks:
      - ai-services-network

  rabbitmq:
    image: rabbitmq:3-management
    environment:
      RABBITMQ_DEFAULT_USER: ${RABBITMQ_USER}
      RABBITMQ_DEFAULT_PASS: ${RABBITMQ_PASSWORD}
    ports:
      - "5672:5672"
      - "15672:15672"
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq
    restart: unless-stopped
    networks:
      - ai-services-network

volumes:
  db_data:
  redis_data:
  rabbitmq_data:

networks:
  ai-services-network:
    driver: bridge
```

### E. Performance Optimization Checklist

- [ ] Enable database connection pooling
- [ ] Implement response caching where appropriate
- [ ] Use Redis for session state
- [ ] Optimize database queries with proper indexes
- [ ] Implement lazy loading for related data
- [ ] Use compression for large payloads
- [ ] Enable CDN for static assets
- [ ] Implement pagination for large datasets
- [ ] Use async/await throughout the application
- [ ] Optimize image delivery (WebP format)
- [ ] Implement database read replicas for scaling reads
- [ ] Use prepared statements for repeated queries
- [ ] Implement request/response batching
- [ ] Optimize serialization (JSON vs Protocol Buffers)

### F. Security Checklist

- [ ] Enable HTTPS everywhere
- [ ] Implement CSRF protection
- [ ] Add rate limiting to all endpoints
- [ ] Validate and sanitize all inputs
- [ ] Use parameterized queries
- [ ] Encrypt sensitive data at rest
- [ ] Implement proper authentication (JWT)
- [ ] Add CORS policies
- [ ] Implement security headers
- [ ] Regular security audits
- [ ] Dependency vulnerability scanning
- [ ] Implement logging and monitoring
- [ ] Use secrets management (Azure Key Vault)
- [ ] Implement password policies
- [ ] Add two-factor authentication
- [ ] Regular backup testing
- [ ] Implement data retention policies
- [ ] Add audit logging
- [ ] Test for SQL injection
- [ ] Test for XSS attacks
- [ ] Test for CSRF attacks
- [ ] Implement API key rotation
- [ ] Add IP whitelisting for admin endpoints

### G. Deployment Checklist

- [ ] Environment variables configured
- [ ] Database migrations applied
- [ ] SSL certificates installed
- [ ] DNS records configured
- [ ] CDN enabled and configured
- [ ] Monitoring and alerting set up
- [ ] Backup strategy tested
- [ ] Rollback plan documented
- [ ] Load testing completed
- [ ] Security scanning completed
- [ ] Documentation updated
- [ ] Team trained on new deployment
- [ ] Smoke tests automated
- [ ] Health check endpoints configured
- [ ] Log aggregation set up
- [ ] Error tracking configured
- [ ] Performance monitoring enabled
- [ ] UAT completed successfully
- [ ] Stakeholder sign-off obtained
- [ ] Deployment schedule communicated
- [ ] Support team notified

---

## References & Resources

### Official Documentation
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Azure Documentation](https://docs.microsoft.com/azure)
- [OpenAI API Documentation](https://platform.openai.com/docs)
- [bKash Merchant API](https://developer.bka.sh)
- [Nagad API Documentation](https://nagad.com.bd)

### Recommended Reading
- "Clean Architecture" by Robert C. Martin
- "Domain-Driven Design" by Eric Evans
- "Building Microservices" by Sam Newman
- "Designing Data-Intensive Applications" by Martin Kleppmann

### Tools & Libraries
- [Serilog](https://serilog.net/)
- [MassTransit](https://masstransit-project.com/)
- [FluentValidation](https://fluentvalidation.net/)
- [AutoMapper](https://automapper.org/)
- [MediatR](https://github.com/jbogard/MediatR)

---

## Document Version History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2026-03-13 | AI Assistant | Initial complete architecture document |

---

**End of Document**