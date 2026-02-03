#!/usr/bin/env bash
# Build and package eWatson NuGet packages

set -e

# Default values
CONFIGURATION="Release"
VERSION=""
CLEAN=true
SKIP_BUILD=false
OUTPUT_DIR="./nupkgs"
PROJECT_PATH="src/eWatson/eWatson.csproj"

# Colors
CYAN='\033[0;36m'
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
GRAY='\033[0;90m'
NC='\033[0m' # No Color

# Helper functions
write_step() {
    echo -e "\n${CYAN}==> $1${NC}"
}

write_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

write_error() {
    echo -e "${RED}✗ $1${NC}"
}

# Usage information
usage() {
    cat << EOF
Usage: $0 [OPTIONS]

Build and package eWatson NuGet packages.

OPTIONS:
    -v, --version VERSION       Override package version
    -c, --configuration CONFIG  Build configuration (Debug|Release, default: Release)
    --no-clean                  Skip cleaning previous packages
    --skip-build                Skip build step, only pack
    -h, --help                  Show this help message

EXAMPLES:
    $0
        Build and pack all projects with Release configuration

    $0 -v 1.0.1-preview
        Build and pack with a specific version

    $0 -c Debug
        Build and pack with Debug configuration

    $0 --skip-build
        Pack without rebuilding
EOF
    exit 0
}

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -v|--version)
            VERSION="$2"
            shift 2
            ;;
        -c|--configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        --no-clean)
            CLEAN=false
            shift
            ;;
        --skip-build)
            SKIP_BUILD=true
            shift
            ;;
        -h|--help)
            usage
            ;;
        *)
            echo "Unknown option: $1"
            usage
            ;;
    esac
done

# No project mapping needed - only packing eWatson

# Ensure output directory exists
if [ ! -d "$OUTPUT_DIR" ]; then
    mkdir -p "$OUTPUT_DIR"
    write_success "Created output directory: $OUTPUT_DIR"
fi

# Clean previous packages
if [ "$CLEAN" = true ]; then
    write_step "Cleaning previous packages"
    rm -f "$OUTPUT_DIR"/*.nupkg
    rm -f "$OUTPUT_DIR"/*.snupkg
    write_success "Cleaned output directory"
fi

# Build solution
if [ "$SKIP_BUILD" = false ]; then
    write_step "Restoring NuGet packages"
    dotnet restore eWatson.slnx
    write_success "Restore completed"

    write_step "Building solution ($CONFIGURATION)"
    BUILD_ARGS=(build eWatson.slnx --configuration "$CONFIGURATION")

    if [ -n "$VERSION" ]; then
        BUILD_ARGS+=("-p:Version=$VERSION")
    fi

    dotnet "${BUILD_ARGS[@]}"
    write_success "Build completed"
fi

# Pack eWatson project
write_step "Creating NuGet package"

PACK_ARGS=(
    pack
    --no-build
    --configuration "$CONFIGURATION"
    --output "$OUTPUT_DIR"
)

if [ -n "$VERSION" ]; then
    PACK_ARGS+=("-p:Version=$VERSION")
fi

echo -e "${YELLOW}Packing eWatson...${NC}"
dotnet pack "$PROJECT_PATH" "${PACK_ARGS[@]}"

if [ $? -ne 0 ]; then
    write_error "Failed to pack eWatson"
    exit 1
fi

write_success "Package created successfully"

# Display created packages
write_step "Created packages"
for package in "$OUTPUT_DIR"/*.nupkg; do
    if [[ ! "$package" == *.symbols.nupkg ]]; then
        filename=$(basename "$package")
        size=$(du -h "$package" | cut -f1)
        echo -e "  ${GRAY}• $filename ($size)${NC}"
    fi
done

echo ""
write_success "Packaging complete! Output: $OUTPUT_DIR"
echo ""
