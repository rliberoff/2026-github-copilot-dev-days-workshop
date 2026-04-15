---
applyTo: "**/*.tf,**/*.tfvars,**/*.tf.json"
description: "Guidelines for generating modern Terraform templates, and coding style conventions for Azure infrastructure. When generating Azure Terraform templates, always follow these guidelines."
---

# Terraform for Azure Guidelines

Guidelines for generating Terraform code for Azure infrastructure following best practices, security standards, and compliance requirements.

## Project Context

- **Terraform Version**: 1.14+ with HCL syntax
- **Primary Providers**: `azurerm` (preferred), `azapi` for cutting-edge features
- **Provider Registry**: [registry.terraform.io](https://registry.terraform.io/)

## General Principles

- Use the Azure knowledge MCP tools (`microsoft_docs_search`, `microsoft_code_sample_search`) to ensure accuracy and best practices in Azure services.
- Use the Terraform MCP Server tools to generate the Terraform code to meet the refined requirements for Azure infrastructure.
- Refine all infrastructure requirements to be Azure-specific and aligned with best practices, security, and compliance standards.
- Keep the infrastructure stack lean and avoid unnecessary complexity.

## Core Rules

- Terraform should be written using HCL (HashiCorp Configuration Language) syntax.
- Ensure that the generated code is well-documented with comments explaining the purpose of each resource and configuration.
- Always try to use implicit dependencies over explicit dependencies where possible.
- When generating Terraform resource names, ensure they are unique and descriptive, lower-case, and snake_case.
- Be sure to include any necessary provider configurations, backend settings, and required variables in the generated code.
- Ensure the generated Terraform code always includes a top level `tags` variable map that is used on all taggable resources, with at least the following tags: `environment`, `project`, and `owner`.
- Ensure that sensitive information such as passwords, API keys, and secrets are not hardcoded. Use variables and values in `.tfvars` files instead.
- Do not assume any prior knowledge about the user's Azure environment; **always seek clarification when in doubt**.
- Before finalizing the Terraform code, always confirm with the user that all requirements have been accurately captured and addressed.
- Any sensitive information in output must be marked as `sensitive = true`.
- Always use the latest version of Terraform unless a specific version is defined or requested.
- Always search in the [Terraform Registry](https://registry.terraform.io/) for the latest provider versions.

## Anti-Patterns to Avoid

### Configuration Anti-Patterns

- **MUST NOT hardcode values** that should be parameterized (e.g., resource names, locations, SKUs).
- **SHOULD NOT use `terraform import`** as a regular workflow pattern. It's for one-time migrations only.
- **SHOULD avoid complex conditional logic** that makes code hard to understand and maintain.
- **MUST NOT use `local-exec` provisioners** unless absolutely necessary; prefer native resource configurations.
- **MUST NOT use `null_resource`** for triggers; use `terraform_data` instead.

### Security Anti-Patterns

- **MUST NEVER store secrets** in Terraform files (`.tf`) or state files.
- **MUST avoid overly permissive IAM roles** or network rules; follow principle of least privilege.
- **MUST NOT disable security features** for convenience (e.g., encryption, firewall rules).
- **MUST NOT use default passwords or keys**; always generate or parameterize credentials.
- **MUST NOT commit `.tfstate` files** or `.terraform/` directories to version control.

### Operational Anti-Patterns

- **MUST only use state files (`*.tfstate`) for read-only operations**; all changes via Terraform CLI or HCL.
- **MUST only use contents of `.terraform/` directory** (fetched modules and providers) for read-only operations.
- **MUST avoid creating separate folders/repos/branches per environment**; use `.tfvars` files for environment differences.

## Provider Selection

- **Use `azurerm` provider** for most scenarios – it offers high stability and covers the majority of Azure services
- **Use `azapi` provider** when `azurerm` lacks support (see **`azapi` Provider Usage** section for details)
- **Document the choice** in code comments when using `azapi`
- Only use official or certified providers:
  - `hashicorp/azuread` for Microsoft Entra ID (former Azure Active Directory) resources
  - `hashicorp/helm` for Helm chart deployments
  - `hashicorp/kubernetes` for Kubernetes resources
  - `hashicorp/random` for random ID generation

## Project Structure and Organization

### Modular Architecture

Use Terraform modules to group reusable infrastructure components. For any resource set that will be used in multiple contexts:

- Create a module with its own variables and outputs
- Reference it rather than duplicating code
- This promotes reuse and consistency
- **Separate modules by resource type**: Each Azure resource type has its own module directory (e.g., `modules/aca/`, `modules/kv/`, `modules/oai/`, `modules/cosmos/`, `modules/acr/`, etc.)

### Organize Code Cleanly

Structure Terraform configurations with logical file separation:

- **Consistent module structure**: Each module contains:
  - `main.tf` - Resource definitions
  - `variables.tf` - Input variables
  - `outputs.tf` - Output values, including sensitive information when required.
  - `providers.tf` - Provider configuration when needed.
- Follow consistent naming conventions and formatting.

### Backend Configuration

- **Separate backend infrastructure**: Backend state storage (`backend/`) is isolated from main resources (`resources/`)
- **Remote state management**: Use Azure Storage Account for Terraform state with proper configuration

## Naming Conventions

### Resource Naming

- **Consistent prefix pattern**: Use Azure resource abbreviations (e.g., `rg-`, `acr`, `appcs-`, `kv-`, `appi-`)
- **Suffix strategy with locals**: Compute resource names with suffixes in locals block, allowing multiple deployments without collisions
- **Random suffix option**: Provide flexibility between random and fixed suffixes via variables
- **Module folders or directory names**: Use kebab-case for module folder names using the same abbreviation strategy

  ```terraform
  locals {
    suffix = lower(trimspace(var.use_random_suffix ? substr(lower(random_id.random.hex), 1, 5) : var.suffix))
    name_resource_group = "${var.resource_group_name}-${local.suffix}"
    name_acr = "${var.acr_name}${local.suffix}"
  }
  ```

### Variable Naming

- **Descriptive prefixes**: Group related variables by resource type (e.g., `aca_bot_*`, `appcs_*`, `openai_*`)
- **Underscores for readability**: Use snake_case for all identifiers

## Variable Management

### Variable Documentation

- **Comprehensive descriptions**: Every variable includes:
  - Required/Optional status (i.e., is nullable or not)
  - Purpose and impact
  - Default values if applicable
  - External references (e.g., Azure documentation links)

### Variable Validation

- **Built-in validation blocks**: Always validate input values at the variable level
- **See Complex Validation Patterns** section for comprehensive validation techniques including:
  - Regex validation with `can(regex())`
  - Range and numeric validation
  - Array uniqueness and element validation
  - Cross-variable validation
  - Conditional required values

### Variable Defaults

- **Parameterize** all configurable values using variables with types and descriptions
- **Sensible defaults**: Provide production-ready defaults (e.g., `location = "swedencentral"`)
- **Explicit nullability**: Use `nullable = false` to enforce required values

## Tagging Strategy

### Consistent Tagging

- **Centralized tag management**: Define common tags in locals:

  ```terraform
  locals {
    tags = merge(var.tags, {
      environment = var.environment
      project     = var.project
      owner       = var.owner
      createdAt   = "${formatdate("YYYY-MM-DD hh:mm:ss", timestamp())} UTC"
      createdWith = "Terraform"
      suffix      = local.suffix
    })
  }
  ```

- **Automatic metadata**: Include creation timestamp and tool information
- **Tag inheritance**: Pass tags to all modules and resources

### Tag Lifecycle

- **Ignore tag changes**: Prevent drift from external tag modifications:

  ```terraform
  lifecycle {
    ignore_changes = [tags]
  }
  ```

## Resource Configuration

### Identity Management

- **User-assigned managed identities**: Prefer user-assigned over system-assigned for flexibility:

  ```terraform
  identity {
    type         = "UserAssigned"
    identity_ids = [var.identity_id]
  }
  ```

- **Consistent identity passing**: Pass managed identity to all resources requiring authentication

### Data Sources

- **Current configuration retrieval**: Use data sources for runtime information:

  ```terraform
  data "azurerm_client_config" "current" {}
  data "azurerm_subscription" "current" {}
  data "azuread_user" "current_user" {
    object_id = data.azurerm_client_config.current.object_id
  }
  ```

### Dynamic Blocks

- **Conditional resource creation**: Use `dynamic` blocks for optional configurations:

  ```terraform
  dynamic "geo_location" {
    for_each = var.geo_locations
    content {
      location          = geo_location.value.location
      failover_priority = geo_location.value.failover_priority
    }
  }
  ```

## Lifecycle Management

### Prevent Unwanted Updates

- For Azure, always ignore changes to `tags`.
- **Selective ignore_changes**: Ignore attributes managed externally:

  ```terraform
  lifecycle {
    ignore_changes = [
      tags,
      template[0].container[0].image,
    ]
  }
  ```

### Trigger-Based Replacement

- **Content-based triggers**: Use `terraform_data` with SHA1 hashing to detect actual file changes:

  ```terraform
  resource "terraform_data" "trigger_update_bot_icon" {
    input = sha1(file_content)
  }

  lifecycle {
    replace_triggered_by = [terraform_data.trigger_update_bot_icon]
  }
  ```

## Security Practices

### Access Control

- **Role-based access**: Define role assignments for managed identities:

  ```terraform
  resource "azurerm_role_assignment" "service_principals_role_assignment" {
    scope                = azurerm_cognitive_account.openai.id
    role_definition_name = "Cognitive Services OpenAI User"
    principal_id         = each.value
  }
  ```

### Secrets Management

- **The best secret is one that does not need to be stored**: Use Managed Identities rather than passwords or keys whenever possible
- **No hardcoded secrets**: Use variables for sensitive values
- **Ephemeral secrets (Terraform v1.11+)**: Use `ephemeral` secrets with write-only parameters to avoid storing secrets in state files:

  ```terraform
  ephemeral "azurerm_key_vault_secret" "admin_password" {
    name         = "admin-password"
    key_vault_id = azurerm_key_vault.main.id
  }

  resource "azurerm_virtual_machine" "example" {
    # ... other configuration
    admin_password = ephemeral.azurerm_key_vault_secret.admin_password.value
  }
  ```

- **When sensitive data or secrets are provided** as part of the development, put them in a `terraform.tfvars` file linked to their corresponding variable
- **Key Vault integration**: Store secrets in Azure Key Vault with proper access policies unless directed to use a different service
- **Name sanitization**: Replace unsupported characters (e.g., `:` to `--` for Key Vault secret names)
- **Never write secrets** to local filesystems or commit to git
- **Mark sensitive values appropriately**: Isolate them from other attributes and avoid outputting sensitive data unless absolutely necessary

## Development Modes

### Environment-Specific Configuration

- **Development mode flag**: Use boolean flag for dev-specific configurations:

  ```terraform
  variable "development_mode" {
    description = "Specifies whether this resource should be created with configurations suitable for development purposes."
    type        = bool
    default     = false
  }
  ```

- **Conditional resource creation**: Use `count` with development mode for dev-only resources:

  ```terraform
  data "azurerm_client_config" "current" {
    count = var.development_mode ? 1 : 0
  }
  ```

## Provider Configuration

### Version Pinning

- **Pessimistic version constraints**: Use `~>` for patch-level flexibility:

  ```terraform
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>4.2.0"
    }
  }
  ```

### Provider Features

- **Bug mitigation flags**: Document workarounds with references:

  ```terraform
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }
  ```

## Module Usage

### Dependency Management

- **Implicit dependencies**: Rely on and prefer Terraform's built-in dependency graph where possible
- **Explicit dependencies**: Use `depends_on` only when implicit dependencies aren't sufficient
- **Redundant depends_on Detection**: Search and remove `depends_on` where the dependent resource is already referenced implicitly in the same resource block:

  ```terraform
  # BAD: Redundant depends_on
  resource "azurerm_subnet" "example" {
    name                 = "subnet-example"
    virtual_network_name = azurerm_virtual_network.example.name  # Implicit dependency
    resource_group_name  = azurerm_resource_group.example.name
    depends_on           = [azurerm_virtual_network.example]    # REDUNDANT!
  }

  # GOOD: Only implicit dependency
  resource "azurerm_subnet" "example" {
    name                 = "subnet-example"
    virtual_network_name = azurerm_virtual_network.example.name  # Implicit dependency is sufficient
    resource_group_name  = azurerm_resource_group.example.name
  }

  # ACCEPTABLE: Explicit dependency when needed
  module "aca_bot_backend" {
    depends_on = [module.acr]  # Module dependency, no implicit reference
    # ... configuration
  }
  ```

- **Never depend on module outputs**: Module outputs create implicit dependencies; explicit `depends_on` is unnecessary

### Module Reusability

- **Use `for_each` for multiple instances**: Create multiple similar resources efficiently:

  ```terraform
  resource "azurerm_cognitive_deployment" "openai" {
    for_each = { for model in var.models : model.id => model }
    # ... configuration
  }
  ```

## Output Management

### Informative Outputs

- **Use outputs** to expose key resource attributes for other modules or user reference.
- **Mark sensitive values** accordingly to protect secrets.
- **Clear descriptions**: Every output has a descriptive explanation.
- **Helper messages**: Include example commands in outputs:

  ```terraform
  output "terraform_init_backend" {
    description = "Shows an example of the Terraform command to initialize a deployment with this backend."
    value       = local.terraform_message
  }
  ```

## Documentation and Comments

### Inline Documentation

- **Complex logic explanations**: Add detailed comments for non-obvious implementations
- **Bug workaround references**: Document known issues with GitHub/Azure issue links
- **Multi-line comment blocks**: Use for detailed explanations of lifecycle rules and special cases

### Code Comments

- **Purpose explanation**: Clarify why certain approaches are taken
- **External references**: Link to official documentation for validation

## File Organization

### Separation of Concerns

- **Split variables by resource**: Use separate variable files when complexity warrants
- **Use `settings_*.tf` files**: For multi-service App Configuration patterns (see **Settings Files Organization** section)
- **Logical grouping**: Group related configurations together

### Regarding `.tfvars` Files

- **Environment-specific values**: Use `.tfvars` files for environment configuration
- **Use tfvars to modify environmental differences**: Aim to keep environments similar whilst cost optimizing for non-production
- **Comments in tfvars**: Document non-obvious values directly in variable files

### Folder Structure Best Practices

Use a consistent folder structure. A suggested structure (never change without user agreement):

```text
my-azure-app/
├── infra/                          # Terraform root module
│   ├── main.tf                     # Core resources
│   ├── variables.tf                # Input variables
│   ├── outputs.tf                  # Outputs
│   ├── providers.tf                # Provider configuration
│   ├── environments/               # Environment-specific configurations
│   │   ├── dev.tfvars              # Development environment
│   │   ├── test.tfvars             # Test environment
│   │   └── prod.tfvars             # Production environment
│   └── modules/                    # Reusable modules
│       ├── kv/                     # Key Vault module
│       │   ├── main.tf
│       │   ├── variables.tf
│       │   └── outputs.tf
│       ├── acr/                    # Container Registry module
│       │   ├── main.tf
│       │   ├── variables.tf
│       │   └── outputs.tf
│       ├── app/                    # App Service module
│       │   ├── main.tf
│       │   ├── variables.tf
│       │   └── outputs.tf
│       └── oai/                    # OpenAI/Cognitive Services module
│           ├── main.tf
│           ├── variables.tf
│           └── outputs.tf
├── .github/workflows/              # CI/CD pipelines
└── README.md                       # Documentation
```

**Anti-pattern**: Avoid branch-per-environment, repository-per-environment, or folder-per-environment layouts that make it hard to test root folder logic between environments

## Testing and Validation

### Pre-Validation Steps

- **Inventory existing resources**: Do an inventory of existing resources and offer to remove unused resource blocks
- **Code review**: Check for anti-patterns, redundant dependencies, and security issues

### Terraform Commands

- **Terraform validate**: Always ensure generated code passes `terraform validate` to check syntax
- **Ask before running plan**: Run `terraform plan` only with explicit user permission
- **Subscription ID**: When running `terraform plan`, source the subscription ID (`subscription_id`) from the `ARM_SUBSCRIPTION_ID` environment variable or ask the user for it to stored it in a `terraform.tfvars` file at root level, **NOT** coded in the provider block:

  ```terraform
  # GOOD: No subscription_id in provider (uses ARM_SUBSCRIPTION_ID env var)
  provider "azurerm" {
    features {}
  }

  # BAD: Hardcoded subscription_id
  provider "azurerm" {
    subscription_id = "12345678-1234-1234-1234-123456789012"  # NEVER DO THIS
    features {}
  }
  ```

- **Environment testing**: Test configurations in non-production environments first
- **Plan review**: Generate and review `terraform plan` output for correctness before applying changes

## Idempotency

- Write configurations that can be applied repeatedly with the same outcome
- **Avoid non-idempotent actions**:
  - Scripts that run on every apply
  - Resources that might conflict if created twice
- **Test by doing multiple `terraform apply` runs** and ensure the second run results in zero changes
- Use resource lifecycle settings or conditional expressions to handle drift or external changes gracefully

## Azure-Specific Best Practices

### Resource Naming and Tagging

- **Follow Azure naming conventions**: Use recommended abbreviations and patterns (see **Naming Conventions** section)
- **Implement consistent tagging**: See **Tagging Strategy** section for tag structure and lifecycle management

### Resource Group Strategy

- **Use existing resource groups when specified**: Query for existing resource groups before creating new ones
- **Create new resource groups only when necessary**: Always confirm with the user
- **Use descriptive names**: Indicate purpose and environment (e.g., `rg-fundraising-dev-abc123`)

### Networking Considerations

- **Validate existing VNet/SubNet IDs** before creating new network resources:
  - Is this solution being deployed into an existing hub & spoke landing zone?
  - Are there existing VNets that should be reused?
  - What are the subnet requirements and CIDR ranges?
- **Use consistent region naming** and variables for multi-region deployments
- **Use Network Security Groups (NSGs) and Application Security Groups (ASGs) appropriately** for network segmentation and security
- **Implement private endpoints** for PaaS services when required
- **Use resource firewall restrictions** to restrict public access; comment exceptions where public endpoints are required
- **Plan for network peering** if multiple VNets are required

### Security and Compliance

- **Use Managed Identities** instead of service principals wherever possible (see **Identity Management** section)
- **Implement Key Vault** with appropriate RBAC for secrets management (see **Secrets Management** section)
- **Enable diagnostic settings** for audit trails on all applicable resources
- **Follow principle of least privilege** for all IAM roles and permissions
- **Enable encryption** at rest and in transit for all applicable resources

### Cost Management

- **Use environment-appropriate sizing**: Dev vs Prod (e.g., B1 for dev, P1v2 for prod)
- **Implement auto-shutdown** for dev/test resources when applicable
- **Use Azure Cost Management tags** for cost tracking and allocation

### State Management

- **Use remote backend**: Azure Storage Account with state locking
- **Implement state locking**: Prevent concurrent modifications
- **Use workspace or environment-specific state files**: Isolate environments

## Application Configuration Patterns

When building multi-service applications, use Azure App Configuration with Key Vault references for centralized configuration management.

### Label-Based Service Configuration

Organize configurations by service using labels. Each service has its own configuration structure with direct values and Key Vault references:

```terraform
locals {
  # Configuration for a specific service
  api_service_config = {
    label = "MyApp.Services.Api"

    # Direct configuration values
    values = {
      "Sentinel"                     = 1
      "AllowedHosts"                 = "*"
      "CacheOptions:ExpirationHours" = "24"
      "DatabaseOptions:MaxRetries"   = "3"
    }

    # References to secrets stored in Key Vault
    keyvault_references = {
      "DatabaseOptions:ConnectionString" = "database-connectionstring"
      "ApiOptions:ApiKey"                = "external-api-key"
    }
  }

  # Shared configuration without label
  common_config = {
    label = ""
    values = {
      "Logging:LogLevel:Default" = "Information"
    }
    keyvault_references = {}
  }
}
```

### Aggregating Multi-Service Configurations

Use nested `for` loops with `flatten()` to aggregate configurations from all services into a single collection for the App Configuration module:

```terraform
locals {
  # Map of all services
  services = {
    common  = local.common_config
    api     = local.api_service_config
    worker  = local.worker_service_config
  }

  # Flatten all values across services
  app_config_values = flatten([
    for service_name, service in local.services : [
      for key, value in service.values : {
        label = service.label
        key   = key
        value = value
      }
    ]
  ])

  # Flatten all Key Vault references across services
  app_config_keyvault_refs = flatten([
    for service_name, service in local.services : [
      for key, secret_name in service.keyvault_references : {
        label       = service.label
        key         = key
        vault_name  = module.kv.name
        secret_name = secret_name
      }
    ]
  ])
}
```

### App Configuration Module Integration

Pass the aggregated configurations to the App Configuration module:

```terraform
module "appcs" {
  source   = "./modules/appcs"
  name     = "appcs-myapp-${var.environment}"
  # ... other configuration
  values   = local.app_config_values
  secrets  = local.app_config_keyvault_refs
}
```

The module iterates over these collections using `for_each`:

```terraform
resource "azurerm_app_configuration_key" "secrets" {
  for_each = { for s in var.secrets : "${s.label}|${s.key}" => s }

  configuration_store_id = azurerm_app_configuration.appcs.id
  key                    = each.value.key
  label                  = each.value.label
  type                   = "vault"
  vault_key_reference    = "https://${each.value.vault_name}.vault.azure.net/secrets/${each.value.secret_name}"
}

resource "azurerm_app_configuration_key" "values" {
  for_each = { for v in var.values : "${v.label}|${v.key}" => v }

  configuration_store_id = azurerm_app_configuration.appcs.id
  key                    = each.value.key
  label                  = each.value.label
  value                  = each.value.value
  type                   = "kv"
}
```

## Settings Files Organization

For complex multi-service infrastructures, organize configuration into separate settings files to improve maintainability.

### File Naming Convention

Use the pattern `settings_<component>.tf` for configuration-focused files:

| File                    | Purpose                                         |
| ----------------------- | ----------------------------------------------- |
| `settings_common.tf`    | Shared configuration values across all services |
| `settings_secrets.tf`   | Centralized secrets mapping to Key Vault        |
| `settings_<service>.tf` | Per-service App Configuration values            |

### Structure Example

```text
resources/
├── main.tf                    # Core resource definitions and module calls
├── variables.tf               # Input variables
├── outputs.tf                 # Output values
├── providers.tf               # Provider configuration
├── settings_common.tf         # Shared config (logging, telemetry)
├── settings_secrets.tf        # Secrets aggregation
├── settings_api.tf            # API service configuration
├── settings_worker.tf         # Worker service configuration
└── modules/
```

### Settings Files Content Pattern

**settings_common.tf** - Shared values without service label:

```terraform
locals {
  common_config = {
    label = ""
    values = {
      "Logging:LogLevel:Default"   = "Information"
      "Logging:LogLevel:Microsoft" = "Warning"
    }
    keyvault_references = {}
  }
}
```

**settings_secrets.tf** - Centralized secrets mapping:

```terraform
locals {
  secrets = {
    # Secrets from module outputs
    "database-connectionstring" = module.cosmos.connection_string
    "storage-connectionstring"  = module.st.connection_string
    "openai-key"                = module.openai.key

    # Secrets from input variables
    "external-api-key" = var.external_api_key
  }
}
```

**settings\_\<service\>.tf** - Per-service configuration:

```terraform
locals {
  api_service_config = {
    label = "MyApp.Services.Api"
    values = {
      "ServiceOptions:Endpoint" = module.cosmos.endpoint
      # ... service-specific values
    }
    keyvault_references = {
      "ConnectionStrings:Database" = "database-connectionstring"
    }
  }
}
```

## `azapi` Provider Usage

Use the `azapi` provider when `azurerm` lacks support for new Azure features or resource types.

### When to Use `azapi`

| Scenario                     | Provider  | Example                                  |
| ---------------------------- | --------- | ---------------------------------------- |
| Standard Azure resources     | `azurerm` | Storage accounts, VNets, App Services    |
| New/preview features         | `azapi`   | RAI policies, dynamic throttling         |
| Unsupported resource types   | `azapi`   | Cutting-edge cognitive services features |
| Updating specific properties | `azapi`   | Post-deployment configuration changes    |

### Provider Configuration

Configure both providers together:

```terraform
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.32.0"
    }
    azapi = {
      source  = "azure/azapi"
      version = "~> 2.4.0"
    }
  }
}
```

### Creating Resources with `azapi_resource`

Use `azapi_resource` for resource types not yet in `azurerm`. Always reference the Azure REST API specification for the correct schema:

```terraform
resource "azapi_resource" "content_filter" {
  type      = "Microsoft.CognitiveServices/accounts/raiPolicies@2024-10-01"
  name      = "CustomPolicy"
  parent_id = azurerm_cognitive_account.openai.id

  # Disable schema validation for preview APIs if needed
  schema_validation_enabled = false

  # Reference: https://github.com/Azure/azure-rest-api-specs
  body = {
    properties = {
      mode           = "Default"
      basePolicyName = "Microsoft.DefaultV2"
      contentFilters = [
        { name = "Violence", blocking = true, enabled = true, severityThreshold = "High", source = "Prompt" },
        { name = "Hate", blocking = true, enabled = true, severityThreshold = "High", source = "Prompt" },
        # Order matters - maintain consistent ordering to prevent drift
      ]
    }
  }
}
```

### Updating Existing Resources with `azapi_update_resource`

Use `azapi_update_resource` to modify properties on resources created by `azurerm`:

```terraform
resource "azapi_update_resource" "enable_dynamic_throttling" {
  for_each = { for model in var.models : model.id => model }

  type      = "Microsoft.CognitiveServices/accounts/deployments@2024-10-01"
  parent_id = azurerm_cognitive_account.openai.id
  name      = each.value.model_name

  body = {
    properties = {
      dynamicThrottlingEnabled = true
      versionUpgradeOption     = "OnceNewDefaultVersionAvailable"
    }
  }

  depends_on = [azurerm_cognitive_deployment.openai]
}
```

### Best Practices for `azapi`

- **Always include API version** in the `type` attribute (e.g., `@2024-10-01`)
- **Reference Azure REST API specs** for correct property names and structures
- **Document the reason** for using `azapi` over `azurerm` in comments
- **Use `depends_on`** to ensure proper ordering with `azurerm` resources
- **Maintain array ordering** in body properties to prevent unnecessary updates
- **Set `schema_validation_enabled = false`** for preview APIs with incomplete schemas

## File-Based Resource Seeding

Populate Azure resources with initial data from local files during deployment.

### Reading CSV Files for Table Storage

Use `file()` to read CSV content and populate Azure Table Storage:

```terraform
locals {
  # Read CSV files from a relative path
  products_csv = file("${path.module}/../../../data/products.csv")
  settings_csv = file("${path.module}/../../../data/settings.csv")
}
```

### Bulk File Discovery with `fileset()`

Use `fileset()` to discover multiple files for blob uploads:

```terraform
locals {
  # Discover all files in a directory
  schema_files = {
    for f in fileset("${path.module}/../../../schemas", "**/*.json") :
    f => "${path.module}/../../../schemas/${f}"
  }

  # Discover specific file types
  query_files = {
    for f in fileset("${path.module}/../../../queries", "*.sql") :
    f => "${path.module}/../../../queries/${f}"
  }
}
```

### Uploading Files to Blob Storage

```terraform
resource "azurerm_storage_blob" "schemas" {
  for_each = local.schema_files

  name                   = each.key
  storage_account_name   = azurerm_storage_account.sa.name
  storage_container_name = azurerm_storage_container.schemas.name
  type                   = "Block"
  source                 = each.value
  content_md5            = filemd5(each.value)
}
```

### Content-Based Change Detection

Use `filesha1()` or `filemd5()` to trigger updates only when file content changes:

```terraform
resource "terraform_data" "schema_trigger" {
  input = sha1(join("", [for f in local.schema_files : filesha1(f)]))
}

resource "azurerm_storage_blob" "config" {
  # ... configuration

  lifecycle {
    replace_triggered_by = [terraform_data.schema_trigger]
  }
}
```

### Conditional File Loading

Check file existence before loading:

```terraform
locals {
  config_path   = "${path.module}/config/settings.json"
  config_exists = fileexists(local.config_path)
  config_data   = local.config_exists ? file(local.config_path) : "{}"
  config_sha1   = local.config_exists ? filesha1(local.config_path) : null
}
```

### Folder Structure for Seed Data

Organize seed data files consistently:

```text
inf/
├── data/
│   ├── tables/           # CSV files for Table Storage
│   │   ├── Products.csv
│   │   └── Settings.csv
│   ├── blobs/            # Files for Blob Storage
│   │   ├── schemas/
│   │   └── templates/
│   └── cosmosdb/         # JSON documents for Cosmos DB
│       └── seed-data.json
└── terraform/
    └── resources/
        └── modules/
            └── st/
                └── main.tf   # References ../../../data/
```

## Complex Validation Patterns

Use advanced validation techniques to enforce input constraints and prevent misconfigurations.

### Cross-Variable Validation

Validate that variable combinations are valid:

```terraform
variable "sku" {
  description = "The SKU tier. Possible values are Free, Standard, Premium."
  type        = string
  default     = "Free"

  validation {
    condition     = contains(["Free", "Standard", "Premium"], var.sku)
    error_message = "SKU must be Free, Standard, or Premium."
  }
}

variable "cost_analysis_enabled" {
  description = "Enable cost analysis. Requires Standard or Premium SKU."
  type        = bool
  default     = false

  validation {
    condition     = var.cost_analysis_enabled ? contains(["Standard", "Premium"], var.sku) : true
    error_message = "Cost analysis requires Standard or Premium SKU."
  }
}
```

### Array Uniqueness Validation

Ensure array elements have unique identifiers:

```terraform
variable "models" {
  description = "List of AI models to deploy."
  type = list(object({
    id            = string
    name          = string
    version       = string
    sku           = string
    capacity      = number
  }))

  validation {
    condition     = length(var.models) == length(distinct([for m in var.models : m.id]))
    error_message = "Each model ID must be unique."
  }
}
```

### Array Element Validation with `alltrue()`

Validate all elements in an array meet criteria:

```terraform
variable "models" {
  # ... type definition

  validation {
    condition = alltrue([
      for model in var.models : contains(
        ["Standard", "GlobalStandard", "ProvisionedManaged"],
        model.sku
      )
    ])
    error_message = "Invalid SKU. Allowed values: Standard, GlobalStandard, ProvisionedManaged."
  }
}
```

### Regex Pattern Validation

Use `can(regex())` for pattern matching:

```terraform
variable "resource_suffix" {
  description = "Suffix for resource names. Must be 3-8 lowercase alphanumeric characters."
  type        = string

  validation {
    condition     = can(regex("^[a-z0-9]{3,8}$", var.resource_suffix))
    error_message = "Suffix must be 3-8 lowercase alphanumeric characters."
  }
}
```

### Numeric Range Validation

Enforce minimum and maximum values:

```terraform
variable "node_count" {
  description = "Number of cluster nodes. Must be between 1 and 100."
  type        = number
  default     = 3

  validation {
    condition     = var.node_count >= 1 && var.node_count <= 100
    error_message = "Node count must be between 1 and 100."
  }
}

variable "retention_days" {
  description = "Log retention in days. Must be 7, 30, 60, 90, or 365."
  type        = number
  default     = 30

  validation {
    condition     = contains([7, 30, 60, 90, 365], var.retention_days)
    error_message = "Retention must be 7, 30, 60, 90, or 365 days."
  }
}
```

### Conditional Required Values

Validate that dependent values are provided:

```terraform
variable "use_existing_resource" {
  description = "Whether to use an existing resource."
  type        = bool
  default     = false
}

variable "existing_resource_id" {
  description = "ID of existing resource. Required when use_existing_resource is true."
  type        = string
  default     = null

  validation {
    condition     = var.use_existing_resource ? var.existing_resource_id != null : true
    error_message = "existing_resource_id is required when use_existing_resource is true."
  }
}
```

### Multiple Validations Per Variable

Apply multiple validation blocks for comprehensive checks:

```terraform
variable "environment" {
  description = "Deployment environment name."
  type        = string

  validation {
    condition     = length(var.environment) >= 2 && length(var.environment) <= 10
    error_message = "Environment name must be 2-10 characters."
  }

  validation {
    condition     = can(regex("^[a-z]+$", var.environment))
    error_message = "Environment name must be lowercase letters only."
  }

  validation {
    condition     = contains(["dev", "test", "staging", "prod"], var.environment)
    error_message = "Environment must be dev, test, staging, or prod."
  }
}
```

## References

- [Abbreviation recommendations for Azure resources](https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-abbreviations)
- [Azure Naming Conventions](https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming)
- [Azure REST API Specifications](https://learn.microsoft.com/en-us/rest/api/azure/)
- [Terraform Best Practices](https://learn.hashicorp.com/collections/terraform/azure-get-started)
