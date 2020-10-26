{
  pkgs ? import <nixpkgs> {},
  delveBinary ? "",
  doCheck ? true
}:
with pkgs;
let
  deps = import ./deps.nix {inherit fetchurl;};
in
stdenv.mkDerivation rec {
  name = "clientSDK-${version}";
  version = "1.1.0";
  buildInputs = [
    dotnet-sdk_3
    dotnetPackages.Nuget
  ];

  src = ./.;

  buildPhase = ''
    mkdir home
    export HOME=$PWD/home

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
    dotnet pack DelveClientSDK/DelveClientSDK.csproj

    mkdir -p $out/{bin,lib}
    mv DelveClientSDK/bin/Debug/DelveClientSDK.${version}.nupkg $out/lib
  '';

  checkPhase = ''
    # running delve server
    dotnet test --no-restore
  '';

  doCheck = false;
}
