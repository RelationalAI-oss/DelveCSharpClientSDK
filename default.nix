{ pkgs ? import <nixpkgs>{}}:
with pkgs;
let
  deps = import ./deps.nix {inherit fetchurl;};
in
stdenv.mkDerivation rec {
  name = "ClientSDK-${version}";
  version = "1.0.0";
  buildInputs = [
    dotnet-sdk_3
    dotnetPackages.Nuget
  ];

  src = ./.;

  buildCommand = ''
    TMPDIR="$(mktemp -d)"
    cp -rv $src/* $TMPDIR
    cd $TMPDIR
    chmod -R +rw .
    
    mkdir home
    export HOME=$PWD/home
    export DOTNET_CLI_TELEMETRY_OPTOUT=1
    export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

    # disable default-source so nuget does not try to download from online-repo
    nuget sources Disable -Name "nuget.org"
    # add all dependencies to source called 'sdk'
    for package in ${toString deps}; do
      nuget add $package -Source sdk
    done

    dotnet restore --source sdk DelveClientSDK.sln
    dotnet build --no-restore -c Release DelveClientSDK.sln
    cd DelveClientSDK
    nuget pack DelveClientSDK.nuspec  -Version 1.0.0

    mkdir -p $out/{bin,lib}
    mv Relational.AI.1.0.0.nupkg $out/lib
  '';
}
