# AKSupport Terraform POC

<!-- BEGIN_TF_DOCS -->
## Requirements

| Name | Version |
|------|---------|
| <a name="requirement_terraform"></a> [terraform](#requirement\_terraform) | >= 1.3.0 |
| <a name="requirement_azurerm"></a> [azurerm](#requirement\_azurerm) | ~> 3.0 |
| <a name="requirement_helm"></a> [helm](#requirement\_helm) | ~> 2.6 |
| <a name="requirement_random"></a> [random](#requirement\_random) | ~> 3.4 |

## Providers

| Name | Version |
|------|---------|
| <a name="provider_azurerm"></a> [azurerm](#provider\_azurerm) | 3.26.0 |
| <a name="provider_helm"></a> [helm](#provider\_helm) | 2.7.0 |
| <a name="provider_random"></a> [random](#provider\_random) | 3.4.3 |

## Modules

| Name | Source | Version |
|------|--------|---------|
| <a name="module_aks"></a> [aks](#module\_aks) | github.com/StevenJDH/Terraform-Modules//azure/aks | main |

## Resources

| Name | Type |
|------|------|
| [azurerm_monitor_action_group.aksupport](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/monitor_action_group) | resource |
| [helm_release.aksupport](https://registry.terraform.io/providers/hashicorp/helm/latest/docs/resources/release) | resource |
| [random_id.suffix](https://registry.terraform.io/providers/hashicorp/random/latest/docs/resources/id) | resource |
| [azurerm_client_config.current](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/data-sources/client_config) | data source |

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:--------:|
| <a name="input_aks_cluster_name"></a> [aks\_cluster\_name](#input\_aks\_cluster\_name) | Name of AKS cluster. | `string` | `""` | no |
| <a name="input_aks_cluster_url"></a> [aks\_cluster\_url](#input\_aks\_cluster\_url) | Optional Azure Portal URL for AKS cluster. | `string` | `""` | no |
| <a name="input_aksupport_namespace"></a> [aksupport\_namespace](#input\_aksupport\_namespace) | Default namespace for AKSupport. | `string` | `"default"` | no |
| <a name="input_aksupport_version"></a> [aksupport\_version](#input\_aksupport\_version) | Version of AKSupport to deploy if latest is not desired or version pinning is needed. | `string` | `"latest"` | no |
| <a name="input_app_id"></a> [app\_id](#input\_app\_id) | App Id (Client Id) of the Service Principal. | `string` | n/a | yes |
| <a name="input_app_password"></a> [app\_password](#input\_app\_password) | Password (Client Secret) of SP. | `string` | n/a | yes |
| <a name="input_app_tenant_id"></a> [app\_tenant\_id](#input\_app\_tenant\_id) | Tenant of SP. | `string` | n/a | yes |
| <a name="input_avatar_image_url"></a> [avatar\_image\_url](#input\_avatar\_image\_url) | Optional URL to an avatar image for Teams and Office Mail notifications. | `string` | `"https://raw.githubusercontent.com/StevenJDH/AKSupport/main/Avatars/aksupport-256x256-transparent-bg.png"` | no |
| <a name="input_cron"></a> [cron](#input\_cron) | Schedule for CronJob. | `string` | `"* * * * *"` | no |
| <a name="input_k8s_version"></a> [k8s\_version](#input\_k8s\_version) | Supported Kubernetes version for AKS. Minor version aliases such as 1.23 are also supported. | `string` | `"1.23"` | no |
| <a name="input_mail_app_id"></a> [mail\_app\_id](#input\_mail\_app\_id) | App Id (Client Id) of the Office Service Principal. | `string` | `""` | no |
| <a name="input_mail_app_password"></a> [mail\_app\_password](#input\_mail\_app\_password) | Password (Client Secret) of Office SP. | `string` | `""` | no |
| <a name="input_mail_app_tenant"></a> [mail\_app\_tenant](#input\_mail\_app\_tenant) | Office 365 tenant of Office SP. | `string` | `""` | no |
| <a name="input_mail_recipient_address"></a> [mail\_recipient\_address](#input\_mail\_recipient\_address) | Email address of recipient. | `string` | `""` | no |
| <a name="input_mail_sender_id"></a> [mail\_sender\_id](#input\_mail\_sender\_id) | Email address or user's Object Id of sender. | `string` | `""` | no |
| <a name="input_region"></a> [region](#input\_region) | Azure region (without spaces) for deploying resources. | `string` | `"West Europe"` | no |
| <a name="input_version_test"></a> [version\_test](#input\_version\_test) | For testing a specific version like 1.17.0. to see what AKSupport does. | `string` | `""` | no |
| <a name="input_webhook_url"></a> [webhook\_url](#input\_webhook\_url) | Teams channel incoming webhook URL. | `string` | `""` | no |

## Outputs

| Name | Description |
|------|-------------|
| <a name="output_action_group_id"></a> [action\_group\_id](#output\_action\_group\_id) | n/a |
| <a name="output_client_certificate"></a> [client\_certificate](#output\_client\_certificate) | n/a |
| <a name="output_fqdn"></a> [fqdn](#output\_fqdn) | n/a |
| <a name="output_host"></a> [host](#output\_host) | n/a |
| <a name="output_kube_config"></a> [kube\_config](#output\_kube\_config) | n/a |
| <a name="output_kubeconfig_cmd"></a> [kubeconfig\_cmd](#output\_kubeconfig\_cmd) | n/a |
<!-- END_TF_DOCS -->