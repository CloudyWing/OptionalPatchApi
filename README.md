# OptionalPatchApi

這是一個 ASP.NET Core Web API 範例，展示 PATCH 端點怎麼區分「沒傳這個欄位」和「明確把欄位設成 null」。

這個寫法純屬個人興趣的實驗，實務上需要部分更新時，建議優先採用業界較通用的作法（如 JSON Patch、JSON Merge Patch），再考慮要不要自製 `OptionalValue<T>` 這類包裝。

實作背景與設計考量可參考這篇筆記：[在 ASP.NET Core Web API 中實現可選更新功能](https://note.cloudywing.net/backend/%E5%9C%A8%20ASP.NET%20Core%20Web%20API%20%E4%B8%AD%E5%AF%A6%E7%8F%BE%E5%8F%AF%E9%81%B8%E6%9B%B4%E6%96%B0%E5%8A%9F%E8%83%BD)。

## 技術重點

- Target Framework：`.NET 10`
- OpenAPI：`Microsoft.AspNetCore.OpenApi 10.0.8`
- 測試：`NUnit 4.6.0` + `NSubstitute 5.3.0`
- 整合測試：`Microsoft.AspNetCore.Mvc.Testing 10.0.8`
- 專案結構：`src` / `tests`

## 功能

- `OptionalValue<T>` 表示可選欄位。
- JSON request body 以一般欄位格式輸入，不暴露 `HasValue` 與 `Value` 包裝結構。
- Form request 使用 ModelBinder 將欄位轉為 `OptionalValue<T>`。
- DataAnnotation 僅在欄位有傳入時才驗證。
- OpenAPI 文件使用 .NET 10 內建 transformer 調整 schema。

## 執行

```powershell
dotnet run --project .\src\OptionalPatchApi\OptionalPatchApi.csproj
```

開發環境會提供 OpenAPI 文件：

```text
/openapi/v1.json
```

## 測試

```powershell
dotnet test .\OptionalPatchApi.slnx
```

## 範例

未傳入任何欄位時，現有資料不會被修改：

```json
{}
```

傳入 `null` 時，表示把欄位明確更新為 null：

```json
{
  "description": null
}
```

傳入不合法欄位時，會回傳 ASP.NET Core 標準 Validation ProblemDetails。

## 授權條款

本專案採用 [MIT License](LICENSE.md)。
