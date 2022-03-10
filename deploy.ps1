$tagDate = Get-Date -Format "yyyyMMddHHmmss"

$attempts = 0
$startSuccess = $false
$maxAttempts = 3

$dockerFile.FullName
	docker build -t brandonlempka/toosimple:$tagDate --platform linux/arm64/v8 .
	docker push brandonlempka/toosimple:$tagDate
