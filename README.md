# Proforma Viewer

Windows desktop application for selecting AT200 or AT300 proformas and combining specified PDF pages into one printable PDF.

## Features
- Loads the latest AT200/AT300 workbook directly from the configured network location.
- Reads `Title`, `Path`, and `Page` columns without requiring Excel.
- Searchable, multi-select tile interface.
- Supports page formats `15`, `16,17`, `1,2`, `16x4`, `20x2`, and `21x2`.
- Combines selected pages, reports missing files/invalid pages, and opens the result.
- Self-contained Windows x64 build through GitHub Actions.

## Database defaults
- `I:\ServiceDelivery\ECR\1. Proforma Viewer\Databases\AT200 Proforma list.xlsx`
- `I:\ServiceDelivery\ECR\1. Proforma Viewer\Databases\AT300 Performa List.xlsx`

Edit `src/ProformaViewer/appsettings.json` if these locations change. UNC paths are also supported. Workbooks and PDFs are intentionally excluded from Git.

## Publish to GitHub
1. Create an empty GitHub repository.
2. Extract this package and upload all contents, including `.github`.
3. Commit to `main`.
4. Open **Actions > Build**. Download `ProformaViewer-win-x64` after the workflow succeeds.
5. For a release, create and push a tag, for example `v1.0.0`. The Release workflow creates `ProformaViewer-win-x64.zip` on the GitHub Releases page.

## Local development
Requirements: Windows and .NET 10 SDK.

```powershell
dotnet restore ProformaViewer.sln
dotnet test ProformaViewer.sln -c Release
dotnet run --project src/ProformaViewer/ProformaViewer.csproj
```

## Production notes
- Users must be connected to the company network and have read access to the workbook/PDF locations.
- If `I:` is not mapped consistently, replace it with the organisation's UNC path.
- Code-sign the final executable before broad deployment if required by company IT policy.
- Password-protected or damaged PDFs will be reported during export.
