# Proforma Viewer v1.3.0

## AT200 frequency selector
The AT200 toolbar contains an X01-X24 selector. Choosing a frequency reads the external `385 Star chart.xlsx` workbook and selects matching VMI tiles. AT300 does not show this selector.

### Editing frequencies
Edit the star chart on the network drive. Keep `VMI Task` in the first column, keep `X01` through `X24` as headers, and place any non-empty marker such as `★` in cells where a task is required. The app reads the workbook each time a frequency is selected, so no code rebuild is required.

Default schedule path:
`I:\ServiceDelivery\ECR\1. Proforma Viewer\Databases\385 Star chart.xlsx`

If the workbook moves, edit `Schedules:AT200` in `src/ProformaViewer/appsettings.json`.

## GitHub deployment
Upload the complete repository, including `.github`, to `main`. The Build workflow produces a self-contained `ProformaViewer-win-x64` artifact.
