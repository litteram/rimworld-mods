{
  description = "rimworld mods";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-24.05";
    flakelight.url = "github:nix-community/flakelight";
  };

  outputs = { self, flakelight, ... }@inputs:
    flakelight ./. ({ lib,...}:{
        inherit inputs;
        systems = lib.systems.flakeExposed;

        devShell.packages = pkgs: with pkgs;[
          libxml2
          lemminx # lsp

          # mono stuff
          ilspycmd
          avalonia-ilspy
          msbuild
          mono
          omnisharp-roslyn # lsp
          dotnet-sdk
          ];
          devShell.env= pkgs: {
          DOTNET_ROOT = "${pkgs.dotnet-sdk}";
        };
    });
}

