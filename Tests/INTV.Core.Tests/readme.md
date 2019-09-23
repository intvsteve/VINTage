# INTV.Core.Tests
The goal of these tests is to provide confidence that interfaes and types behave as documented. The tests also achieve 100% code coverage. New code or changes to it must maintain this.

## Xunit
The tests are writen using using Xunit, so running them will require Xunit 2.0. As this code base still expects to be built using older tools (Visual Studio 2012), older versions of Xunit are required.

## Known Intermittent Failures
INTV.Core has not adopted a fully formalized dependency injection model. That means that there are a few areas where the tests attempt to inject dependencies is not always successful when running all tests. The most common failure points involves tests that depend on `IStorageAccess`.

Generally speaking these failures occur due collisions in the injected `IStorageAccess` or caching of it by `StorageLocation`.

Put another way: Bad globals! **_Bad!_** :P
