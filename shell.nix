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

    # steam uploading
    steamcmd
  ];

  env = {
  };
}
