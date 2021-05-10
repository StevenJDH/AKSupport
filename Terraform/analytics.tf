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

resource "random_id" "suffix" {
  byte_length = 8

  count = var.prep_for_azure_monitor_alert ? 1 : 0
}

resource "azurerm_log_analytics_workspace" "oms" {
  # The workspace name has to be globally unique.
  name                = "log-aksupport-LogAnalyticsWorkspace-${random_id.suffix[0].dec}-${var.stage}"
  location            = var.region
  resource_group_name = azurerm_resource_group.aksupport.name
  sku                 = var.log_analytics_workspace_sku

  tags = var.tags
  count = var.prep_for_azure_monitor_alert ? 1 : 0
}

resource "azurerm_log_analytics_solution" "container-insights" {
  solution_name         = "ContainerInsights" # Must be same name as 'product' in plan below.
  location              = azurerm_log_analytics_workspace.oms[0].location
  resource_group_name   = azurerm_resource_group.aksupport.name
  workspace_resource_id = azurerm_log_analytics_workspace.oms[0].id
  workspace_name        = azurerm_log_analytics_workspace.oms[0].name

  plan {
    publisher = "Microsoft"
    product   = "OMSGallery/ContainerInsights"
  }

  count = var.prep_for_azure_monitor_alert ? 1 : 0
}