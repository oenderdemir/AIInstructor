# AIInstructor

## Turizm Öğrencileri İçin Senaryo Bazlı Öğrenme Modülü

Bu proje, turizm ve otelcilik öğrencilerinin müşteri ilişkileri ve profesyonel iletişim becerilerini geliştirmelerini sağlayan yapay zekâ destekli bir eğitim altyapısı sağlar. Sistem, OpenAI Chat API ile entegre çalışan bir AI Tutor ve simüle müşteri karakteri üzerinden öğrencilere gerçekçi senaryolar sunar.

### Öne Çıkan Özellikler
- **Senaryo Yönetimi:** Senaryo tanımları `jsonFiles/scenarios` klasöründe JSON formatında tutulur ve uygulama açılışında yüklenir.
- **AI Tabanlı Diyalog:** Her senaryoda AI Tutor, öğrenciyi yönlendirir, hataları nazikçe düzeltir ve diyalog akışını sürdürür.
- **Performans Değerlendirme:** Senaryo sonunda iletişim, problem çözme, dil kullanımı ve profesyonellik kriterlerinde 0-100 arası puanlama yapılır.
- **Oyunlaştırma:** Öğrenciler performanslarına göre puan, seviye ve rozetler kazanır. Profil bilgilerine API üzerinden erişilebilir.

### API Uç Noktaları
- `GET /api/TrainingScenarios` – Senaryo özetlerini listeler.
- `POST /api/TrainingScenarios/{scenarioId}/sessions` – Yeni bir senaryo oturumu başlatır.
- `POST /api/TrainingScenarios/sessions/{sessionId}/student-message` – Öğrencinin mesajını AI Tutor'a iletir.
- `POST /api/TrainingScenarios/sessions/{sessionId}/complete` – Oturumu erken sonlandırır ve değerlendirme oluşturur.
- `GET /api/TrainingScenarios/sessions/{sessionId}/transcript` – Diyalog kayıtlarını ve varsa değerlendirmeyi döndürür.
- `GET /api/TrainingScenarios/students/{studentId}/profile` – Öğrencinin oyunlaştırma profilini verir.

### Yapılandırma
`appsettings.json` dosyasında OpenAI API anahtarı ve senaryo veri dizini tanımlanmalıdır:
```json
"OpenAI": {
  "ApiKey": "YOUR_OPENAI_API_KEY",
  "BaseUrl": "https://api.openai.com/",
  "DefaultModel": "gpt-4.1-mini"
},
"ScenarioData": {
  "Directory": "jsonFiles/scenarios"
}
```

### Geliştirme Notları
1. OpenAI API anahtarınızı güvenli bir şekilde `appsettings.Development.json` veya kullanıcı gizli anahtarlarında saklayın.
2. Yeni senaryoları JSON formatında ekleyerek eğitim içeriğini genişletebilirsiniz.
3. Gerekirse değerlendirme istemini `EvaluationService` içinde özelleştirerek farklı rubrikler kullanabilirsiniz.

