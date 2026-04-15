---
applyTo: "**/*.cs"
description: "Programming rules designed to help writing cleaner, more maintainable, and more object-oriented code."
---

# Object Calisthenics Guidelines

Guidelines for writing cleaner, more maintainable, and more object-oriented C# code based on the original 9 Object Calisthenics rules.

> **Warning**: This file contains the 9 original Object Calisthenics rules. No additional rules must be added, and none of these rules should be replaced or removed.

## Project Context

- **Target Framework**: .NET 10 with C# 14
- **Origin**: [Object Calisthenics by Jeff Bay](https://www.cs.helsinki.fi/u/luontola/tdd-2009/ext/ObjectCalisthenics.pdf)
- **Purpose**: Enforce clean code principles for domain and application layers

## The 9 Rules

### 1. Maximum three levels of indentation per method

Ensure methods are simple and do not exceed three levels of indentation.

```csharp
// BAD - This method has more than three levels of indentation
public void ProcessOrders() {
    foreach (var order in orders) {                        // Level 1
        if (order.IsValid) {                               // Level 2
            foreach (var item in order.Items) {            // Level 3
                if (item.RequiresShipping) {               // Level 4 - TOO DEEP!
                    shippingService.Ship(item);
                }
            }
        }
    }
}

// GOOD - Three levels of indentation (acceptable)
public void SendNewsletter() {
    foreach (var user in users) {                          // Level 1
        if (user.IsActive) {                               // Level 2
            foreach (var subscription in user.Subscriptions) { // Level 3
                mailer.Send(user.Email, subscription.Topic);
            }
        }
    }
}

// GOOD - Extracted method to reduce deep nesting
public void ProcessOrders() {
    foreach (var order in orders) {
        if (order.IsValid) {
            ProcessOrderItems(order.Items);
        }
    }
}

private void ProcessOrderItems(IEnumerable<OrderItem> items) {
    foreach (var item in items) {
        if (item.RequiresShipping) {
            shippingService.Ship(item);
        }
    }
}

// GOOD - Using LINQ to reduce nesting
public void SendNewsletter() {
    var activeUsers = users.Where(user => user.IsActive);

    foreach (var user in activeUsers) {
        foreach (var subscription in user.Subscriptions) {
            mailer.Send(user.Email, subscription.Topic);
        }
    }
}
```

### 2. Don't Use the ELSE Keyword

- Use Guard Clauses to validate inputs and conditions at the beginning of methods.
- Avoid using the `else` keyword to reduce complexity and improve readability.
- Use early returns to handle conditions instead.

  ```csharp
  // BAD - Using else
  public void ProcessOrder(Order order) {
      if (order.IsValid) {
          // Process order
      } else {
          // Handle invalid order
      }
  }

  // GOOD - Avoiding else
  public void ProcessOrder(Order order) {
      if (!order.IsValid) return;

      // Process order
  }
  ```

Use the 'Fail Fast' principle:

```csharp
   public void ProcessOrder(Order order) {
       if (order == null) throw new ArgumentNullException(nameof(order));
       if (!order.IsValid) throw new InvalidOperationException(@"Invalid order");

       // Process order
   }
```

Using the `CommunityToolkit.Diagnostics` package is strongly recommended for robust and consistent argument validation:

```csharp
   using CommunityToolkit.Diagnostics;

   public void ProcessOrder(Order order) {
       Guard.IsNotNull(order, nameof(order));
       Guard.IsTrue(order.IsValid, nameof(order), @"Invalid order");

       // Process order
   }
```

### 3. Wrapping all primitives and strings when thet represent domain concepts with business logic

- Avoid using primitive types directly in your code when they represent domain concepts.
- Wrap them in classes to provide meaningful context and behavior (business logic).

  ```csharp
  // BAD - Using primitive types directly
  public class User {
      public string Name { get; set; }
      public int Age { get; set; }
  }

  // GOOD - Wrapping primitives
  public class User {
      public User(string name, Age age) {
          this.name = name;
          this.age = age;
      }

      public string Name { get; init; }

      public Age Age { get; init; }
  }

  public class Age {
      private int value;

      public Age(int value) {
          if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), @"Age cannot be negative");
          this.value = value;
      }
  }
  ```

### 4. First class collections

- Use collections to encapsulate data and behavior, rather than exposing raw data structures.
- Prefer interfaces like `IList<T>`, `IEnumerable<T>`, or custom collection classes to manage collections.

```csharp
// BAD - Exposing raw collection
public class Group {
    public int Id { get; init; }
    public string Name { get; init; }
    public List<User> Users { get; init; }

    public int GetNumberOfUsersIsActive()
    {
        return Users.Where(user => user.IsActive).Count();
    }
}

// GOOD - Encapsulating collection behavior
public class Group {
    public int Id { get; init; }
    public string Name { get; init; }

    public GroupUserCollection UserCollection { get; init; } // The list of users is encapsulated in a class

    public int GetNumberOfUsersIsActive() {
        return UserCollection.GetActiveUsers().Count();
    }
}

// The `GroupUserCollection` class encapsulates the collection of users and provides methods to interact with it.

public class GroupUserCollection {
    private List<User> users;

    public GroupUserCollection(IList<User> users) {
        this.users = users;
    }

    public IEnumerable<User> GetActiveUsers() {
        return users.Where(user => user.IsActive);
    }
}
```

### 5. When extracting entities or models, prefer "one dot per line" as long as it makes sense

When accessing entities or models properties limit the number of method calls in a single line to improve readability and maintainability.

```csharp
// GOOD - Multiple dots in a single line
public void ProcessOrder(Order order)
{
    var userEmail = order.User.GetEmail().ToUpper().Trim();
    // Do something with userEmail
}

// BETTER - One dot per line
public void ProcessOrder(Order order)
{
    var user = order.User;
    var email = user.GetEmail();
    var userEmail = email.ToUpper().Trim();
    // Do something with userEmail
}
```

### 6. Avoid abbreviations

- Use meaningful names for classes, methods, and variables.
- Do not use abbreviations that can lead to confusion.

  ```csharp
  // BAD - Abbreviated names
  public class U {
      public string N { get; set; }
  }

  // GOOD - Meaningful names
  public class User {
      public string Name { get; set; }
  }
  ```

### 7. Keep entities small

- Limit the size of classes and methods to improve code readability and maintainability.
- Each class should have a single responsibility and be as small as possible. Constraints:
  - Maximum 10 methods per class.
  - Maximum 50 lines per class.
  - Prefer a maximum of 10 classes per package or namespace.

  ```csharp
  // GOOD - A Manager/Service class with cohesive CRUD operations is acceptable
  public class UserManager {
      public void CreateUser(string name) { /*...*/ }
      public void UpdateUser(int id, string name) { /*...*/ }
      public void DeleteUser(int id) { /*...*/ }
      public User GetUser(int id) { /*...*/ }
  }

  // BAD - Class mixing unrelated responsibilities
  public class OrderProcessor {
      public void ProcessOrder(Order order) { /*...*/ }
      public void SendConfirmationEmail(string email) { /*...*/ }  // Email responsibility
      public byte[] GeneratePdfInvoice(Order order) { /*...*/ }    // PDF generation responsibility
      public void UpdateInventory(Order order) { /*...*/ }         // Inventory responsibility
      public void CalculateShippingCost(Order order) { /*...*/ }   // Shipping responsibility
  }

  // GOOD - Separated into focused, single-responsibility classes
  public class OrderProcessor {
      public void ProcessOrder(Order order) { /*...*/ }
  }

  public class OrderNotificationService {
      public void SendConfirmationEmail(string email) { /*...*/ }
  }

  public class InvoiceGenerator {
      public byte[] GeneratePdfInvoice(Order order) { /*...*/ }
  }

  public class InventoryService {
      public void UpdateInventory(Order order) { /*...*/ }
  }

  public class ShippingCalculator {
      public decimal CalculateShippingCost(Order order) { /*...*/ }
  }
  ```

### 8. No classes with more than seven instance variables

- Encourage classes to have a single responsibility by limiting the number of instance variables.
- Limit the number of instance variables to no more than seven to maintain simplicity.
- Do not count `ILogger` or any other logger or telemetry as instance variable.

  ```csharp
  // BAD - Class with multiple instance variables
  public class UserCreateCommandHandler {
     private readonly IAuditService auditService;
     private readonly IBroadcastService broadcastService;
     private readonly IEmailService emailService;
     private readonly ILogger logger;
     private readonly IMessageService messageService;
     private readonly INotificationService notificationService;
     private readonly IQueueService queueService;
     private readonly ISmsService smsService;
     private readonly IUserRepository userRepository;

     public UserCreateCommandHandler(IAuditService auditService,
          IBroadcastService broadcastService,
          IEmailService emailService,
          IMessageService messageService,
          INotificationService notificationService,
          IQueueService queueService,
          ISmsService smsService,
          IUserRepository userRepository,
          ILogger logger)
      {
          this.auditService = auditService;
          this.broadcastService = broadcastService;
          this.emailService = emailService;
          this.messageService = messageService;
          this.notificationService = notificationService;
          this.queueService = queueService;
          this.smsService = smsService;
          this.userRepository = userRepository;
          this.logger = logger;
     }
  }

  // GOOD - Class with few instance variables
  public class UserCreateCommandHandler {
     private readonly IUserRepository userRepository;
     private readonly INotificationService notificationService;
     private readonly ILogger logger; // This is not counted as instance variable

     public UserCreateCommandHandler(IUserRepository userRepository, INotificationService notificationService, ILogger logger) {
        this.userRepository = userRepository;
        this.notificationService = notificationService;
        this.logger = logger;
     }
  }

  // GOOD - Encapsulating related dependencies in a record
  public record OrderProcessingServices(
      IOrderRepository OrderRepository,
      IPaymentGateway PaymentGateway,
      IInventoryService InventoryService,
      IShippingCalculator ShippingCalculator,
      INotificationService NotificationService
  );

  public class OrderCommandHandler {
     private readonly OrderProcessingServices services;
     private readonly ILogger logger;

     public OrderCommandHandler(OrderProcessingServices services, ILogger logger) {
        this.services = services;
        this.logger = logger;
     }

     public void ProcessOrder(Order order) {
        services.OrderRepository.Save(order);
        services.PaymentGateway.Charge(order.Total);
        services.InventoryService.Reserve(order.Items);
        services.ShippingCalculator.Calculate(order);
        services.NotificationService.SendConfirmation(order);
     }
  }
  ```

### 9. Avoid getters and Setters in domain classes

```csharp
// BAD
public class User {
    public string Name { get; set; } // Avoid this
}

// GOOD
public class Person {
    public Peron() {};

    public Person(string name) {
        Name = name;
    }

    public string Name { get; init; }
}
```

## Implementation guidelines

- **Domain Classes**:
  - When possible or convinient, use private constructors and static factory methods for creating instances.
  - Avoid exposing setters for properties. Prefer `init` properties or methods that modify state.
  - Apply all 9 rules strictly for business domain code.

- **Application Layer**:
  - Apply these rules to use case handlers and application services.
  - Focus on maintaining single responsibility and clean abstractions.

- **DTOs and Data Objects**:
  - Rules 3 (wrapping primitives), 8 (two instance variables), and 9 (no getters/setters) may be relaxed for DTOs.
  - Public properties with getters or setters are acceptable for data transfer objects and configuration options.

- **Testing**:
  - Ensure tests validate the behavior of objects rather than their state.
  - Test classes may have relaxed rules for readability and maintainability.

- **Code Reviews**:
  - Enforce these rules during code reviews for domain and application code.
  - Be pragmatic about infrastructure and DTO code.

> **When in doubt, prefer the simpler approach, ask the user or request better specifications**
