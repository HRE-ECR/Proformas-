# Deployment checklist

1. Keep company Excel/PDF documents out of the repository.
2. Verify paths in `appsettings.json`.
3. Push to GitHub and confirm the Build action passes.
4. Download the build artifact and test on a standard user workstation.
5. Confirm network permissions and mapped-drive availability.
6. Code-sign the executable for managed deployment where required.
7. Tag `v1.0.0` to create the first release.

The published build is self-contained. Users do not need to install the .NET runtime separately.
