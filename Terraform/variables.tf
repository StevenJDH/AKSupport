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

variable "stage" {
  description = "Name of stage."
  default = "poc"
}

variable "k8s_version" {
  description = "Supported Kubernetes version for AKS."
  default = "1.19.9"
}

variable "log_analytics_workspace_sku" {
  description = "Azure Log Analytics workspace SKU."
  default     = "PerGB2018" # Reference: https://azure.microsoft.com/pricing/details/monitor.
}

variable "cron" {
  description = "Schedule for CronJob."
  default = "* * * * *"
}

variable "tags" {
  description = "Tags for Azure resources."
  type = map(string)
  default = {
    Environment = "poc"
  }
}

########################################################################
# Base AKSupport Configuration                                         #
########################################################################

variable "subscription_id" {
  description = "Subscription Id of the SP and where Azure resource will deploy."
}

variable "app_tenant_id" {
  description = "Tenant of SP."
}

variable "app_id" {
  description = "App Id (Client Id) of the Service Principal."
}

variable "app_password" {
  description = "Password (Client Secret) of SP."
  sensitive = true
}

variable "region" {
  description = "Azure region (without spaces) for deploying resources."
  default = "westeurope"
}

variable "version_test" {
  description = "For testing a specific version like 1.17.0. to see what AKSupport does."
  type = list(string)
  default = []
}

# All other feature requirements are validated by AKSupport.
variable "prep_for_azure_monitor_alert" {
  description = "Enables Azure Monitor integration, and uses mail_recipient_address for Action Group."
  type = bool
  default = true
}

########################################################################
# Teams Configuration                                                  #
########################################################################

variable "webhook_url" {
  description = "Teams channel incoming webhook URL."
  default = ""
  sensitive = true
}

########################################################################
# Teams and Office Mail Configuration                                  #
########################################################################

variable "avatar_image_url" {
  description = "Optional URL to an avatar image for Teams and Office Mail notifications."
  default = "https://raw.githubusercontent.com/StevenJDH/AKSupport/main/Avatars/aksupport-256x256-transparent-bg.png"
}

variable "aks_cluster_name" {
  description = "Name of AKS cluster."
  default = ""
}

variable "aks_cluster_url" {
  description = "Optional Azure Portal URL for AKS cluster."
  default = ""
}

########################################################################
# Office Mail Configuration                                            #
########################################################################

variable "mail_app_tenant" {
  description = "Office 365 tenant of Office SP."
  default = ""
}

variable "mail_app_id" {
  description = "App Id (Client Id) of the Office Service Principal."
  default = ""
}

variable "mail_app_password" {
  description = "Password (Client Secret) of Office SP."
  default = ""
  sensitive = true
}

variable "mail_sender_id" {
  description = "Email address or user's Object Id of sender."
  default = ""
}

# Requires a value if prep_for_azure_monitor_alert is true.
variable "mail_recipient_address" {
  description = "Email address of recipient."
  default = ""
}