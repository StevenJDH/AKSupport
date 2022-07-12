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
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AKSupport.Models;

public record Email
{
    [JsonPropertyName("message")]
    public Message? NewMessage { get; init; }

    [JsonPropertyName("saveToSentItems")] 
    public string SaveToSentItems { get; init; } = "false";
}

public record Message
{
    [JsonPropertyName("subject")]
    public string? Subject { get; init; }

    [JsonPropertyName("body")]
    public Body? Body { get; init; }

    [JsonPropertyName("toRecipients")]
    public ToRecipient[]? ToRecipients { get; init; }
}

public record Body
{
    [JsonPropertyName("contentType")]
    public string? ContentType { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }
}

public record ToRecipient
{
    [JsonPropertyName("emailAddress")]
    public EmailAddress? EmailAddress { get; init; }
}

public record EmailAddress
{
    [JsonPropertyName("address")]
    public string? Address { get; init; }
}
