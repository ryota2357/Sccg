all: test build packages

test:
	@dotnet test --configuration Release

build:
	@dotnet build --configuration Release

packages:
	@dotnet pack --configuration Release -o packages

clean:
	@rm -rf packages

.PHONY: test build packages