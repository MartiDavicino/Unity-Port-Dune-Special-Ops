<?php

// BBDD connection
$servername = "127.0.0.1";
$username = "aitoram1";
$password = "rFXcLYvwKB";
$dbname = "aitoram1";
$port = "3306";

// Create connection
$conn = mysqli_connect($servername, $username, $password, $dbname);

// Check connection
if (!$conn) {
    die("Connection failed: " . mysqli_connect_error());
}
$sql = "SELECT PosID, x, z FROM Positions ";

$result = $conn->query($sql);

if($result->num_rows > 0)
{
    $rows = array();
    while($row = $result->fetch_assoc())
    {
        $rows[] = $row;
        echo $row["PosID"]. "," . $row["x"] . "," . $row["z"] ."*";
    }
    //echo json_encode($rows);
}

$conn->close();

?>

