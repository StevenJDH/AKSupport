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

resource "azurerm_monitor_action_group" "aksupport" {
  name                = "ag-aksupport-${var.stage}"
  resource_group_name = azurerm_resource_group.aksupport.name
  short_name          = "aksupport"

  email_receiver {
    email_address = var.mail_recipient_address
    name          = "aksupport"
  }

  count = var.prep_for_azure_monitor_alert ? 1 : 0
}

# Custom log alerts based on a Container Insights query still not available:
# https://github.com/terraform-providers/terraform-provider-azurerm/issues/4395