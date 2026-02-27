using System.Net.Http.Json;
using System.Text.Json;

namespace Raven_Family.Services
{
    public class DiscordService
    {
        private readonly HttpClient _httpClient;
        private readonly string _webhookUrl = "https://discord.com/api/webhooks/1466733361820209276/CpmcCXnTxZdd-KWJxyFpsGqzi9fjGo-ZMZghgh4m8R63sGTHIT9yEbl2TogOVmaXttTq";
        private readonly string _botName = "🐦⬛ Raven Family Bot";
        private readonly string _botAvatarUrl = "https://fs39.fex.net/preview/5818024514/400x0";

        public DiscordService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SendApplicationAsync(ApplicationData applicationData)
        {
            try
            {
                var embed = CreateApplicationEmbed(applicationData);
                var payload = new DiscordWebhookPayload
                {
                    Username = _botName,
                    AvatarUrl = _botAvatarUrl,
                    Embeds = new[] { embed }
                };

                var response = await _httpClient.PostAsJsonAsync(_webhookUrl, payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Discord webhook error: {ex.Message}");
                return false;
            }
        }

        private DiscordEmbed CreateApplicationEmbed(ApplicationData data)
        {
            return new DiscordEmbed
            {
                Title = "🐦‍⬛ Нова Заявка на Вступ",
                Description = $"Користувач **{data.Nickname}** подав заявку для вступу до Raven Family",
                Color = 0xC41E3A, // Red (#C41E3A)
                Fields = new[]
                {
                    new DiscordEmbedField
                    {
                        Name = "👤 Базова Інформація",
                        Value = $"**Нікнейм:** {data.Nickname}\n" +
                                $"**Статік:** {data.Statistics}\n" +
                                $"**Час на день:** {data.DailyTime}\n" +
                                $"**Мікрофон + Discord:** {data.MicDiscord}",
                        Inline = false
                    },
                    new DiscordEmbedField
                    {
                        Name = "🎮 Ігровий Досвід",
                        Value = $"```{TruncateText(data.GameExperience, 200)}```",
                        Inline = false
                    },
                    new DiscordEmbedField
                    {
                        Name = "🏢 Історія в Інших Сім'ях",
                        Value = $"**Попередні сім'ї:**\n```{TruncateText(data.PreviousFamilies, 150)}```\n" +
                                $"**Причина виходу:**\n```{TruncateText(data.ReasonForLeaving, 150)}```\n" +
                                $"**Покарання/Бани:** {data.Punishments}",
                        Inline = false
                    },
                    new DiscordEmbedField
                    {
                        Name = "📝 Як Дізналися про Нас",
                        Value = data.HowFound,
                        Inline = true
                    },
                    new DiscordEmbedField
                    {
                        Name = "❤️ Чому Саме Ми?",
                        Value = $"```{TruncateText(data.WhyJoinUs, 200)}```",
                        Inline = false
                    },
                    new DiscordEmbedField
                    {
                        Name = "💪 Сильні Сторони та Стиль Гри",
                        Value = $"**Сильні сторони RP:**\n```{TruncateText(data.Strengths, 150)}```\n" +
                                $"**Стиль гри:** {data.PlayStyle}",
                        Inline = false
                    },
                    new DiscordEmbedField
                    {
                        Name = "⚖️ Розуміння Правил та Дисципліни",
                        Value = $"**Ставлення до правил:** {data.RulesAttitude}\n" +
                                $"**Готів нести відповідальність:** {data.Responsibility}\n" +
                                $"**Готів виконувати завдання:** {data.MissionsWithoutReward}",
                        Inline = false
                    },
                    new DiscordEmbedField
                    {
                        Name = "🎯 Ситуаційні Запитання",
                        Value = $"**Лідер наказав відступити (але шанс виграти):**\n```{TruncateText(data.LeaderOrderVsChance, 150)}```\n" +
                                $"**Порушення правил іншим членом:**\n```{TruncateText(data.RuleBreakerAction, 150)}```",
                        Inline = false
                    },
                    new DiscordEmbedField
                    {
                        Name = "🪦 Лояльність та Цінності",
                        Value = $"**Вірність сім'ї:** {data.Loyalty}\n" +
                                $"**Символ ворона:** {data.RavenSymbol}\n" +
                                $"**Наказ vs Вигода:** {TruncateText(data.OrderVsProfit, 100)}",
                        Inline = false
                    },
                    new DiscordEmbedField
                    {
                        Name = "📱 Discord",
                        Value = $"```{data.Discord}```",
                        Inline = true
                    }
                },
                Footer = new DiscordEmbedFooter
                {
                    Text = "Raven Family | Система заявок",
                    IconUrl = "https://fs39.fex.net/preview/5818024514/400x0"
                },
                Timestamp = DateTime.UtcNow.ToString("O")
            };
        }

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "—";
            if (text.Length <= maxLength) return text;
            return text.Substring(0, maxLength) + "...";
        }
    }

    public class ApplicationData
    {
        public string Nickname { get; set; } = "";
        public string Statistics { get; set; } = "";
        public string DailyTime { get; set; } = "";
        public string MicDiscord { get; set; } = "";
        public string GameExperience { get; set; } = "";
        public string PreviousFamilies { get; set; } = "";
        public string ReasonForLeaving { get; set; } = "";
        public string Punishments { get; set; } = "";
        public string HowFound { get; set; } = "";
        public string WhyJoinUs { get; set; } = "";
        public string FamilyMeaning { get; set; } = "";
        public string YourBenefit { get; set; } = "";
        public string Strengths { get; set; } = "";
        public string PlayStyle { get; set; } = "";
        public string HierarchyReady { get; set; } = "";
        public string RulesAttitude { get; set; } = "";
        public string Responsibility { get; set; } = "";
        public string MissionsWithoutReward { get; set; } = "";
        public string LeaderOrderVsChance { get; set; } = "";
        public string RuleBreakerAction { get; set; } = "";
        public string AmbushScenario { get; set; } = "";
        public string OrderVsProfit { get; set; } = "";
        public string Loyalty { get; set; } = "";
        public string RavenSymbol { get; set; } = "";
        public string Discord { get; set; } = "";
    }

    public class DiscordWebhookPayload
    {
        [System.Text.Json.Serialization.JsonPropertyName("username")]
        public string Username { get; set; } = "";

        [System.Text.Json.Serialization.JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; } = "";

        [System.Text.Json.Serialization.JsonPropertyName("embeds")]
        public DiscordEmbed[] Embeds { get; set; } = Array.Empty<DiscordEmbed>();
    }

    public class DiscordEmbed
    {
        [System.Text.Json.Serialization.JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [System.Text.Json.Serialization.JsonPropertyName("color")]
        public int Color { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("fields")]
        public DiscordEmbedField[] Fields { get; set; } = Array.Empty<DiscordEmbedField>();

        [System.Text.Json.Serialization.JsonPropertyName("footer")]
        public DiscordEmbedFooter? Footer { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = "";
    }

    public class DiscordEmbedField
    {
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [System.Text.Json.Serialization.JsonPropertyName("value")]
        public string Value { get; set; } = "";

        [System.Text.Json.Serialization.JsonPropertyName("inline")]
        public bool Inline { get; set; }
    }

    public class DiscordEmbedFooter
    {
        [System.Text.Json.Serialization.JsonPropertyName("text")]
        public string Text { get; set; } = "";

        [System.Text.Json.Serialization.JsonPropertyName("icon_url")]
        public string IconUrl { get; set; } = "";
    }
}
