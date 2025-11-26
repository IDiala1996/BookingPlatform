var builder = DistributedApplication.CreateBuilder(args);

// ===== Shared Services =====
var cache = builder.AddRedis("cache");

// ===== Microservices =====

// Users Service
var usersService = builder.AddProject<Projects.UsersService_API>("users-service")
    ;

// Providers Service
var providersService = builder.AddProject<Projects.ProvidersService_API>("providers-service")
  ;

// Bookings Service
var bookingsService = builder.AddProject<Projects.BookingsService_API>("bookings-service")
    ;

// Reviews Service
var reviewsService = builder.AddProject<Projects.ReviewsService_API>("reviews-service")
  ;

// Notifications Service
var notificationsService = builder.AddProject<Projects.NotificationsService_API>("notifications-service")
  ;

// Payments Service
var paymentsService = builder.AddProject<Projects.PaymentsService_API>("payments-service")
   ;



builder.Build().Run();




