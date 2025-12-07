<?php

// Check if there is POST data
if ($_SERVER['REQUEST_METHOD'] === 'POST') {

    date_default_timezone_set('America/Tijuana');

    // Generate a daily directory name using the current date
    $date = date('Y-m-d');
    $dailyDirectory = $date;

    // Check if the directory exists
    $isNewDirectory = false;
    if (!file_exists($dailyDirectory)) {
        mkdir($dailyDirectory, 0777, true);
        $isNewDirectory = true;  // Flag that a new directory was created
    }

    // Generate a unique file name using the current timestamp
    $timestamp = date('H-i-s');
    $filePath = "$dailyDirectory/data_$timestamp.txt";

    // Read raw POST data from the input stream in case of JSON content
    $rawPostData = file_get_contents("php://input");

    // Decode URL-encoded data
    $decodedData = urldecode($rawPostData);

    // Attempt to decode the JSON
    $postData = json_decode($decodedData, true);

    // Check if json_decode found valid JSON, if not, try traditional $_POST
    if (is_null($postData) && !empty($decodedData)) {
        $postData = $decodedData;  // Use decoded data if it's not JSON
    } elseif (empty($postData)) {
        $postData = $_POST;  // Fallback to $_POST if no data was JSON-decoded
    }

    // Check if $postData is still empty and handle the case
    if (empty($postData)) {
        echo 'No POST data found';
        return;
    }

    // Encode data to JSON, pretty print
    $jsonData = json_encode($postData, JSON_PRETTY_PRINT | JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE);

    // Open the file for writing; create it if it doesn't exist
    $file = fopen($filePath, 'w');

    // Write the JSON data to the file
    fwrite($file, $jsonData . PHP_EOL);

    // Close the file
    fclose($file);

    // Send an email if a new directory was created
    if ($isNewDirectory) {
        $to = 'davidbarile@gmail.com';
        $subject = 'Mage Wars Export';
        $message = 'Go check the server, David.';
        $headers = 'From: MageWars.com' . "\r\n" .
                   'Reply-To: davidbarile@gmail.com' . "\r\n" .
                   'X-Mailer: PHP/' . phpversion();

        mail($to, $subject, $message, $headers);
    }

    // Respond to the client
    echo 'Data saved successfully in ' . $filePath;
} else {
    // Inform the user that no POST data was received
    echo 'No POST data received';
}

?>
