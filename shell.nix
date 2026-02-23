{
  pkgs ? import <nixpkgs> { },
}:
pkgs.mkShell {
  packages = with pkgs; [
    bsdbuild
    libxml2
    lemminx # lsp

    # mono stuff
    ilspycmd
    avalonia-ilspy
    avalonia
    omnisharp-roslyn

    msbuild
    dotnet-sdk
    mono4
    mono6

    # steam uploading
    steamcmd
  ];

  env = {
  };
}
