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

variable "k8s_version" {
  description = "Supported Kubernetes version for AKS. Minor version aliases such as 1.23 are also supported."
  type        = string
  default     = "1.23"
}

########################################################################
# Base AKSupport Configuration                                         #
########################################################################

variable "aksupport_version" {
  description = "Version of AKSupport to deploy if latest is not desired or version pinning is needed."
  type        = string
  default     = "latest"
}

variable "aksupport_namespace" {
  description = "Default namespace for AKSupport."
  type        = string
  default     = "default"
}

variable "cron" {
  description = "Schedule for CronJob."
  type        = string
  default     = "* * * * *"
}

variable "app_tenant_id" {
  description = "Tenant of SP."
  type        = string
}

variable "app_id" {
  description = "App Id (Client Id) of the Service Principal."
  type        = string
}

variable "app_password" {
  description = "Password (Client Secret) of SP."
  type        = string
  sensitive   = true
}

variable "region" {
  description = "Azure region (without spaces) for deploying resources."
  type        = string
  default     = "West Europe"
}

variable "version_test" {
  description = "For testing a specific version like 1.17.0. to see what AKSupport does."
  type        = string
  default     = ""
}

########################################################################
# Teams Configuration                                                  #
########################################################################

variable "webhook_url" {
  description = "Teams channel incoming webhook URL."
  type        = string
  default     = ""
  sensitive   = true
}

########################################################################
# Teams and Office Mail Configuration                                  #
########################################################################

variable "avatar_image_url" {
  description = "Optional URL to an avatar image for Teams and Office Mail notifications."
  type        = string
  default     = "https://raw.githubusercontent.com/StevenJDH/AKSupport/main/Avatars/aksupport-256x256-transparent-bg.png"
}

variable "aks_cluster_name" {
  description = "Name of AKS cluster."
  type        = string
  default     = ""
}

variable "aks_cluster_url" {
  description = "Optional Azure Portal URL for AKS cluster."
  type        = string
  default     = ""
}

########################################################################
# Office Mail Configuration                                            #
########################################################################

variable "mail_app_tenant" {
  description = "Office 365 tenant of Office SP."
  type        = string
  default     = ""
}

variable "mail_app_id" {
  description = "App Id (Client Id) of the Office Service Principal."
  type        = string
  default     = ""
}

variable "mail_app_password" {
  description = "Password (Client Secret) of Office SP."
  type        = string
  default     = ""
  sensitive   = true
}

variable "mail_sender_id" {
  description = "Email address or user's Object Id of sender."
  type        = string
  default     = ""
}

# Requires a value if prep_for_azure_monitor_alert is true.
variable "mail_recipient_address" {
  description = "Email address of recipient."
  type        = string
  default     = ""
}