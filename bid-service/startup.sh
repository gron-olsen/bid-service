export server="localhost"
export port="27017"
export database="auctionDB"
export collection="auctionBidCol"
export rabbitmqUrl="localhost"
export rabbitmqPort="5672"
export mongodb="mongodb://localhost:27017/"
echo $database $BidCollection
dotnet run server="$server" port="$port" collection="$collection" database="$database" rabbitmqUrl=$rabbitmqUrl rabbitmqPort=$rabbitmqPort mongodb=$mongodb
#chmod +x ./startup.sh
#./startup.sh