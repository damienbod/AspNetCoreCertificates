# Certificate Manager change log

## 2024-10-31 version 1.0.9
Updated packages and dependencies

## 2022-01-21 version 1.0.8
Update serial conversion to use big endian only 

## 2022-01-16 version 1.0.7
* Improved support for chained certificates
* Added new methods to support creating RSA Device Chained Certificates
* Updated packages

## 2020-11-21 version 1.0.6
* Add support for all SubjectAlternativeName properties

## 2020-11-21 
* Updated packages, move to .NET 6

## 2020-11-08 
* Updated packages

## 2020-08-08 version 1.0.5
* OidLookup for common OIDs
* The proj files have been updated to enable SourceLink 
* Feature proposal to utilise certificate requests
* Updated Nuget packages and examples

## 2020-02-22 version 1.0.4
* bug fix Subject Key Identifier

## 2020-01-29 version 1.0.3
* private key pem exports
* private key import with certificate

## 2020-01-27 version 1.0.2
* Small fixes for RSA certificates KeySize
* IdentityServer example certificates

## 2020-01-24 version 1.0.1
* Support RSA certificates
* Add example for creating development certificates which can be used in SPA development

## 2020-01-22 version 1.0.0
* creates client server certificate authentication certificates
* support for chained certificates: root, intermediate, client, server, device, leaf 
* support for self signed certificates
* Device certificates for Azure IoT Hub with chain: .cer .pem .pfx exports
* Exports .cer .pem .pfx files
