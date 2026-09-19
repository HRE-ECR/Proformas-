# Proforma Viewer v2.0.0

## Local AT200 frequency file

The AT200 X01-X24 selector reads `385 Star chart.xlsx` from the same folder as `ProformaViewer.exe`. It does not use a network schedule path and does not read `Schedules:AT200` from configuration.

The GitHub build automatically publishes these files together:

- `ProformaViewer.exe`
- `385 Star chart.xlsx`
- `appsettings.json`

## Editing frequencies

Use the **Edit frequency file** button on the AT200 page, or open `385 Star chart.xlsx` beside the executable. Keep the `VMI Task` column and the `X01` through `X24` headings. Any non-empty marker means the task is selected for that frequency. Save the workbook and choose the frequency again.

## Important

Download and extract the complete `ProformaViewer-win-x64` GitHub Actions artifact. Do not copy only the EXE, because the editable star chart must remain beside it.

## Build

Upload the complete repository, including `.github`, then push to `main`. The Build workflow creates the self-contained Windows x64 artifact.
