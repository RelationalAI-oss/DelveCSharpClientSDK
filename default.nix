{ pkgs ? import <nixpkgs>{}}:
with pkgs;
let
  deps = import ./deps.nix {inherit fetchurl;};
in
stdenv.mkDerivation rec {
  name = "clientSDK-${version}";
  version = "1.0.0";
  buildInputs = [
    dotnet-sdk_3
    dotnetPackages.Nuget
  ];

  src = ./.;

  buildPhase = ''
    mkdir home

    # disable default-source so nuget does not try to download from online-repo
    nuget sources Disable -Name "nuget.org"
    # add all dependencies to source called 'sdk'
    for package in ${toString deps}; do
      nuget add $package -Source sdk
    done

    dotnet restore --source sdk DelveClientSDK.sln
    dotnet build --no-restore -c Release DelveClientSDK.sln
  '';

  installPhase = ''
    cd DelveClientSDK
    dotnet pack

    mkdir -p $out/{bin,lib}
    mv bin/Debug/DelveClientSDK.1.0.0.nupkg $out/lib
  '';
}
