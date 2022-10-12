/*
 * This file is part of AKSupport <https://github.com/StevenJDH/AKSupport>.
 * Copyright (C) 2021-2022 Steven Jenkins De Haro.
 *
 * AKSupport is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * AKSupport is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with AKSupport.  If not, see <http://www.gnu.org/licenses/>.
 */

resource "random_id" "suffix" {
  byte_length = 4
}

module "aks" {
  source = "github.com/StevenJDH/Terraform-Modules//azure/aks?ref=main"

  cluster_name                 = "aks-aksupport-${local.stage}"
  kubernetes_version           = "1.23"
  create_resource_group        = true
  resource_group_name          = local.resource_group_name
  location                     = "West Europe"
  dns_prefix                   = "public-aks"

  create_log_analytics_workspace_and_container_insights = true
  log_analytics_workspace_name = "log-aks-aksupport-workspace-${random_id.suffix.hex}-${local.stage}"

  network_profile      = local.network_profile
  default_node_pool    = local.default_pool
  secondary_node_pools = []

  tags = local.tags
}

resource "helm_release" "aksupport" {
  name       = "aksupport"
  repository = "https://StevenJDH.github.io/helm-charts"
  chart      = "aksupport"
  version    = var.aksupport_version == "latest" ? null : var.aksupport_version
  namespace  = var.aksupport_namespace
  atomic     = true

  set {
    name  = "image.tag"
    value = var.aksupport_version
  }

  set {
    name  = "cronjob.schedule"
    type  = "string"
    value = var.cron
  }

  set {
    name  = "configMaps.azureSubscriptionId"
    value = data.azurerm_client_config.current.subscription_id
  }

  set {
    name  = "configMaps.azureAppTenant"
    value = var.app_tenant_id
  }

  set {
    name  = "configMaps.azureAksRegion"
    value = var.region
  }

  set {
    name  = "configMaps.azureAppId"
    value = var.app_id
  }

  set {
    name  = "secrets.azureAppPassword"
    value = var.app_password
  }

  set {
    name  = "testVersion"
    type  = "string"
    value = var.version_test
  }

  set {
    name  = "secrets.teamsChannelWebhookUrl"
    value = var.webhook_url
  }

  set {
    name  = "configMaps.avatarImageUrl"
    value = var.avatar_image_url
  }

  set {
    name  = "configMaps.azureAksClusterName"
    value = var.aks_cluster_name
  }

  set {
    name  = "configMaps.azureAksClusterUrl"
    value = var.aks_cluster_url
  }

  set {
    name  = "configMaps.mailAppTenant"
    value = var.mail_app_tenant
  }

  set {
    name  = "configMaps.mailAppId"
    value = var.mail_app_id
  }

  set {
    name  = "secrets.mailAppPassword"
    value = var.mail_app_password
  }

  set {
    name  = "configMaps.mailSenderId"
    value = var.mail_sender_id
  }

  set {
    name  = "configMaps.mailRecipientAddress"
    value = var.mail_recipient_address
  }

  depends_on = [module.aks]
}

resource "azurerm_monitor_action_group" "aksupport" {
  count = var.mail_recipient_address != "" ? 1 : 0

  name                = "ag-aksupport-${local.stage}"
  resource_group_name = local.resource_group_name
  short_name          = "aksupport"

  email_receiver {
    email_address = var.mail_recipient_address
    name          = "aksupport"
  }

  tags = local.tags

  depends_on = [module.aks]
}

# Custom log alerts based on a Container Insights query still not available:
# https://github.com/terraform-providers/terraform-provider-azurerm/issues/4395