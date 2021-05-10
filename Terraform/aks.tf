/**
 * This file is part of AKSupport <https://github.com/StevenJDH/AKSupport>.
 * Copyright (C) 2021 Steven Jenkins De Haro.
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

resource "azurerm_resource_group" "aksupport" {
  name     = "rg-aksupport-${var.stage}"
  location = var.region

  tags = var.tags
}

resource "azurerm_kubernetes_cluster" "k8s" {
  name                = "aks-aksupport-${var.stage}"
  location            = azurerm_resource_group.aksupport.location
  resource_group_name = azurerm_resource_group.aksupport.name
  dns_prefix          = "aksupport-k8s-dns-${var.stage}"
  kubernetes_version  = var.k8s_version

  default_node_pool {
    name       = "minion"
    node_count = 1
    vm_size    = "Standard_D2_v2"
  }

  role_based_access_control {
    enabled = true
  }

  service_principal {
    client_id     = var.app_id
    client_secret = var.app_password
  }

  addon_profile {
    oms_agent {
      enabled                    = var.prep_for_azure_monitor_alert ? true : false
      log_analytics_workspace_id = var.prep_for_azure_monitor_alert ? azurerm_log_analytics_workspace.oms[0].id : null
    }
  }

  network_profile {
    network_plugin     = "kubenet"
    network_policy     = "calico"
    service_cidr       = "172.77.0.0/16"
    docker_bridge_cidr = "172.16.0.1/16"
    dns_service_ip     = "172.77.0.10"
  }

  lifecycle {
    ignore_changes = [
      # Ignore changes to tags, e.g. because a management agent
      # updates these based on some ruleset managed elsewhere.
      tags, default_node_pool.0.node_count, kubernetes_version
    ]
  }

  tags = var.tags
}



