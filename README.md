# Proforma Viewer v1.1.0
A Windows WPF application that reads the AT200 and AT300 network workbooks and combines selected PDF pages.

## Updated interface
- Click anywhere on a tile to select or deselect it.
- Selected tiles use a blue border, blue background, and tick badge.
- Search has been removed.
- Compact Change database control at the top right.
- Folder icon opens the folder containing the active database workbook.
- Export remains at the bottom right.

## Database locations
Configured in `src/ProformaViewer/appsettings.json`.

## GitHub deployment
1. Upload all repository contents, including `.github`, to the `main` branch.
2. Open Actions and run Build, or push a commit.
3. Download the `ProformaViewer-win-x64` artifact.
4. Create a tag such as `v1.1.0` to generate a GitHub Release ZIP.

Network workbooks and PDFs are not included and must remain outside GitHub. Users need access to the mapped `I:` drive or equivalent configured UNC path.
