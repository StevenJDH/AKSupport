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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKSupport.Models;

public record EnvironmentConfig
{
    public string? SubscriptionId { get; init; }
    public string? AppTenant { get; init; }
    public string? AppId { get; init; }
    public string? AppPassword { get; init; }
    public string? AksRegion { get; init; }
    public string? ImageUrl { get; init; }
    public string? AksClusterName { get; init; }
    public string? AksClusterUrl { get; init; }
    public string? ChannelWebhookUrl  { get; init; }
    public string? MailAppTenant { get; init; }
    public string? MailAppId { get; init; }
    public string? MailAppPassword { get; init; }
    public string? SenderId { get; init; }
    public string? RecipientAddress { get; init; }
}
