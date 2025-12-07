{
  description = "rimworld mods";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
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
        ];
    });
}

