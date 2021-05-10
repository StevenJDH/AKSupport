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

resource "kubernetes_secret" "aksupport" {
  metadata {
    name = "aksupport-secret"
    labels = {
      app = "aksupport"
    }
  }
  type = "Opaque"
  data = {
    AZURE_APP_PASSWORD          = var.app_password
    # Teams configuration.
    TEAMS_CHANNEL_WEBHOOK_URL   = var.webhook_url
    # Office Mail configuration.
    MAIL_APP_PASSWORD           = var.mail_app_password
  }
}

resource "kubernetes_config_map" "aksupport" {
  metadata {
    name = "aksupport-config"
    labels = {
      app = "aksupport"
    }
  }
  data = {
    AZURE_SUBSCRIPTION_ID   = var.subscription_id
    AZURE_APP_TENANT        = var.app_tenant_id
    AZURE_APP_ID            = var.app_id
    AZURE_AKS_REGION        = var.region
    # Teams and Office Mail configuration.
    AVATAR_IMAGE_URL        = var.avatar_image_url
    AZURE_AKS_CLUSTER_NAME  = var.aks_cluster_name
    AZURE_AKS_CLUSTER_URL   = var.aks_cluster_url
    # Office Mail configuration.
    MAIL_APP_TENANT         = var.mail_app_tenant
    MAIL_APP_ID             = var.mail_app_id
    MAIL_SENDER_ID          = var.mail_sender_id
    MAIL_RECIPIENT_ADDRESS  = var.mail_recipient_address
  }
}

resource "kubernetes_cron_job" "aksupport" {
  metadata {
    name = "aksupport-cronjob"
    labels = {
      app = "aksupport"
    }
  }
  spec {
    schedule = var.cron
    successful_jobs_history_limit = 1
    failed_jobs_history_limit = 1
    job_template {
      metadata {}
      spec {
        completions = 1
        backoff_limit = 0
        active_deadline_seconds = 120
        template {
          metadata {
            annotations = {
              # Using annotation approach since newer approach is not yet supported by provider.
              "seccomp.security.alpha.kubernetes.io/pod" = "runtime/default"
            }
          }
          spec {
            security_context {
              run_as_non_root = true
              run_as_user = 10101
            }
            container {
              name = "aksupport"
              image = "public.ecr.aws/stevenjdh/aksupport"
              # For testing a specific version.
              args = var.version_test
              image_pull_policy = "Always"
              env {
                name = "AZMON_COLLECT_ENV" # Required to prevent sensitive data leaking to Azure Monitor Logs.
                value = "FALSE"
              }
              env_from {
                config_map_ref {
                  name = kubernetes_config_map.aksupport.metadata[0].name
                }
              }
              env_from {
                secret_ref {
                  name = kubernetes_secret.aksupport.metadata[0].name
                }
              }
              resources {
                requests {
                  memory = "256Mi"
                  cpu = "500m"
                }
                limits {
                  memory = "512Mi"
                  cpu = "1000m"
                }
              }
              volume_mount {
                mount_path = "/tmp"
                name = "tmp"
              }
              security_context {
                read_only_root_filesystem = true # Causes 'Failed to create CoreCLR, HRESULT: 0x80004005', so fix using tmp volume mount.
                allow_privilege_escalation = false
                capabilities {
                  drop = [ "ALL" ]
                }
              }
            }
            volume {
              name = "tmp"
              empty_dir {}
            }
            restart_policy = "Never"
          }
        }
      }
    }
  }

  depends_on = [
    kubernetes_secret.aksupport,
    kubernetes_config_map.aksupport,
  ]
}