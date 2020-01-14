Install openssl and run from the cmd: // Change the CN as required for your domains

set OPENSSL_CONF=c:\OpenSSL-Win64\bin\openssl.cfg

echo Generate CA key: openssl genrsa -passout pass:1111 -des3 -out ca2.key 4096

echo Generate CA certificate: openssl req -passin pass:1111 -new -x509 -days 365 -key ca2.key -out ca2.crt -subj "/C=US/ST=CA/L=Cupertino/O=damienbod1/OU=YourApp/CN=damiengrpc"

echo Generate server key: openssl genrsa -passout pass:1111 -des3 -out server2.key 4096

echo Generate server signing request: openssl req -passin pass:1111 -new -key server2.key -out server2.csr -subj "/C=US/ST=CA/L=Cupertino/O=damienbod1/OU=YourApp/CN=localhost"

echo Self-sign server certificate: openssl x509 -req -passin pass:1111 -days 365 -in server2.csr -CA ca2.crt -CAkey ca2.key -set_serial 01 -out server2.crt

echo Remove passphrase from server key: openssl rsa -passin pass:1111 -in server2.key -out server2.key

echo Generate client key openssl genrsa -passout pass:1111 -des3 -out client2.key 4096

echo Generate client signing request: openssl req -passin pass:1111 -new -key client2.key -out client2.csr -subj "/C=US/ST=CA/L=Cupertino/O=damienbod1/OU=YourApp/CN=localhost"

echo Self-sign client certificate: openssl x509 -passin pass:1111 -req -days 365 -in client2.csr -CA ca2.crt -CAkey ca2.key -set_serial 01 -out client2.crt

echo Remove passphrase from client key: openssl rsa -passin pass:1111 -in client2.key -out client2.key

openssl pkcs12 -export -in server2.crt -inkey server2.key -out server2.pfx

openssl pkcs12 -export -in client2.crt -inkey client2.key -out client2.pfx