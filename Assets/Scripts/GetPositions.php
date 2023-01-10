<?php

// BBDD connection
$servername = "127.0.0.1";
$username = "aitoram1";
$password = "rFXcLYvwKB";
$dbname = "aitoram1";
$port = "3306";

$itemID = 2;

// Create connection
$conn = mysqli_connect($servername, $username, $password, $dbname);

// Check connection
if (!$conn) {
    die("Connection failed: " . mysqli_connect_error());
}
echo "Connect succesfully";

$sql = "SELECT PosID, x, z FROM Positions";

$result = $conn->query($sql);

if($result->num_rows > 0)
{
    $rows = array();
    while($row = $result->fetch_assoc())
    {
        $rows[] = $row;
        //echo "PosID: ". $row["PosID"]. "x: " . $row["x"] . "y" . $row["z"] . "<br>";
    }
    echo json_encode($rows);
}
else
{
    echo "0 results";
}

$conn->close();

?>

