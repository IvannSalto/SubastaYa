# Cargamos los tokens de los diferentes usuarios
TOKENS=(
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjMiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJjb21wcmFkb3IyQHRlc3QuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IkNvbXByYWRvciBIYWJpbGl0YWRvIiwiZXhwIjoxNzg5OTU4NzE1LCJpc3MiOiJTdWJhc3RhWWFBUEkiLCJhdWQiOiJTdWJhc3RhWWFGcm9udGVuZCJ9.bPMJGz8f7BJy6A_m9BF8jT-J0bYVc0FMjWbAxzNE40c"
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJzYWx0b0BnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSXZhbiBTYWx0byIsImV4cCI6MTc4OTk1ODcwMywiaXNzIjoiU3ViYXN0YVlhQVBJIiwiYXVkIjoiU3ViYXN0YVlhRnJvbnRlbmQifQ.oDYqShpqcI3L9dRI14oXuUrK_U51qagOVLX_bumtKVk"
)

AUCTION_ID=1
URL="https://localhost:7281/api/Auction/$AUCTION_ID/bid"
BID_AMOUNT=50000

echo "Disparando pujas simultáneas de distintos usuarios..."

# Recorremos la lista de usuarios
for TOKEN in "${TOKENS[@]}"; do
   curl -k -s -i -X POST "$URL" \
        -H "Authorization: Bearer $TOKEN" \
        -H "Content-Type: application/json" \
        -d "{\"amount\": $BID_AMOUNT}" &
done

wait
echo -e "\nPrueba multi-usuario finalizada"
