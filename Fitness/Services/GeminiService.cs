using System.Text;
using System.Text.Json;

namespace Fitness.Services
{
    public class GeminiService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly string _visionApiUrl;
        private readonly ILogger<GeminiService> _logger;

        public GeminiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"] ?? "";
            // Güncel model: gemini-2.0-flash (veya gemini-1.5-flash-latest)
            _apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
            _visionApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
            _logger = logger;

            // API key kontrolü
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("Gemini API Key yapılandırılmamış!");
            }
        }

        public async Task<string> GetEgzersizOnerisiAsync(string hedef, int yas, decimal kilo, decimal boy, string cinsiyet)
        {
            try
            {
                var prompt = $@"
Sen profesyonel bir fitness antrenoru ve beslenme uzmanisin. Asagidaki bilgilere gore kisisellestirilmis, detayli bir egzersiz ve beslenme programi olustur:

KULLANICI BILGILERI:
- Hedef: {hedef}
- Yas: {yas}
- Kilo: {kilo} kg
- Boy: {boy} cm
- Cinsiyet: {cinsiyet}

LUTFEN SU FORMATTA CEVAP VER:

1. VUCUT ANALIZI
   - BMI hesapla ve yorumla
   - Hedefe uygunluk durumu

2. HAFTALIK EGZERSIZ PROGRAMI (3-4 Gun)
   - Her gun icin detayli egzersiz listesi
   - Set/tekrar sayilari
   - Hangi kas gruplari calisilacak

3. BESLENME ONERILERI
   - Gunluk kalori ihtiyaci
   - Makro besin dagilimi (protein/karbonhidrat/yag)
   - Ornek ogun plani

4. MOTIVASYON VE IPUCLARI

Tum cevabi Turkce ve anlasilir sekilde yaz. Profesyonel ama samimi bir dil kullan.
";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 2048
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiUrl}?key={_apiKey}", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<GeminiResponse>(responseJson);
                    var aiResponse = result?.candidates?[0]?.content?.parts?[0]?.text;

                    if (!string.IsNullOrEmpty(aiResponse))
                    {
                        return aiResponse;
                    }
                    else
                    {
                        _logger.LogWarning("Gemini AI bos yanit dondu");
                        return "AI yanit olusturamadi. Lutfen tekrar deneyin.";
                    }
                }
                else
                {
                    _logger.LogError($"Gemini API Error: {response.StatusCode} - {responseJson}");
                    return $"AI servisi hatasi: {response.StatusCode}\n\nDetay: {responseJson}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini Service Error");
                return $"Sistem hatasi: {ex.Message}";
            }
        }

        public async Task<string> GetGorselAnalizAsync(byte[] imageData, string hedef)
        {
            try
            {
                string base64Image = Convert.ToBase64String(imageData);

                var prompt = $@"
Sen bir fitness ve vucut analizi uzmanissin. Bu goruntudeki kisinin fiziksel durumunu profesyonel olarak analiz et:

KULLANICI HEDEFI: {hedef}

LUTFEN SU FORMATTA DETAYLI ANALIZ YAP:

1. VUCUT TIPI ANALIZI
   - Vucut tipi (ectomorph/mesomorph/endomorph)
   - Genel fiziksel durum
   - Guclu bolgeler
   - Gelisme gerektiren bolgeler

2. HEDEFE OZEL PROGRAM (Haftada 3-4 Gun)
   - Gunlere gore egzersiz plani
   - Set/tekrar sayilari
   - Odaklanilmasi gereken hareketler

3. BESLENME STRATEJISI
   - Hedefe uygun kalori alimi
   - Makro besin dagilimi
   - Ogun zamanlama onerileri

4. 3 AYLIK HEDEF VE MOTIVASYON

Tum analizi Turkce, profesyonel ama samimi bir dille yaz. Gorseldeki kisiye ozel oneriler ver.
";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new object[]
                            {
                                new { text = prompt },
                                new
                                {
                                    inline_data = new
                                    {
                                        mime_type = "image/jpeg",
                                        data = base64Image
                                    }
                                }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 2048
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_visionApiUrl}?key={_apiKey}", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<GeminiResponse>(responseJson);
                    var aiResponse = result?.candidates?[0]?.content?.parts?[0]?.text;

                    if (!string.IsNullOrEmpty(aiResponse))
                    {
                        return aiResponse;
                    }
                    else
                    {
                        _logger.LogWarning("Gemini Vision AI bos yanit dondu");
                        return "Gorsel analizi yapilamadi. Lutfen farkli bir gorsel deneyin.";
                    }
                }
                else
                {
                    _logger.LogError($"Gemini Vision API Error: {response.StatusCode} - {responseJson}");
                    return $"Gorsel analizi hatasi: {response.StatusCode}\n\nDetay: {responseJson}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini Vision Service Error");
                return $"Gorsel analizi sirasinda hata: {ex.Message}";
            }
        }

        private class GeminiResponse
        {
            public Candidate[]? candidates { get; set; }
        }

        private class Candidate
        {
            public Content? content { get; set; }
        }

        private class Content
        {
            public Part[]? parts { get; set; }
        }

        private class Part
        {
            public string? text { get; set; }
        }
    }
}
