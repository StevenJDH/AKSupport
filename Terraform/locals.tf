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

locals {
  stage               = "poc"
  resource_group_name = "rg-aksupport-${local.stage}"

  network_profile = {
    network_plugin = "kubenet"
    network_policy = "calico"
  }

  default_pool = {
    name    = "default"
    vm_size = "Standard_D2_v2"
  }

  tags = {
    environment = local.stage
    terraform   = true
  }
}