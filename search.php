<?php 
	session_start(); 
	$target_dir = "profilepics/";
	 $conn = mysqli_connect('localhost', 'id5748548_localhost', 'fordaboys', 'id5748548_registration');
?>
<html>
<head>
	<title>User Search </title>
	<link rel="stylesheet" type="text/css" href="style.css">
	<link rel="shortcut icon" type="image/x-icon" href="favicon.ico" />
	<style>
table {
font-family: arial, sans-serif;
border-collapse: collapse;
width: 100%;
}

td, th {
border: apx solid #dddddd;
text-align: left;
padding: 8px;
height:60px;

vertical-align: middle;
}

tr:nth-child(even) {
background-color: #dddddd;
}
</style>
</head>
<body>
	<div class="header">
		<h2>Search</h2>
		 	 <form action="/search.php" style="all: initial; width:80%; vertical-align: middle;">
      <input style="width:80%" type="text" placeholder="Search.." name="keyword">
      <button type="submit">Submit</button>
    </form>
	</div>
	<div class="topnav">

</div>

	<div class="content">

	    <table>
<tr>
<th style="height:20px">User</th>
</tr>
<?php 
$sql = "SELECT username FROM users where username like '%" . $_GET['keyword'] . "%' ORDER BY username LIMIT 10";
         if ( $result = mysqli_query($conn,$sql))
         {
    
        while ($row = mysqli_fetch_assoc($result)) {
            if (file_exists ( $target_dir . $row['username']))
            {
         echo '<tr>
<td><img src="'. $target_dir . $row['username'] .'"alt="lol" style="height:60px;width:60px;vertical-align:middle"> <a href="https://freedomaccount.000webhostapp.com/user.php?username='.$row['username'] . '">' .  $row['username'] . '</a></td>
      </tr>';
}else{
      echo '<tr>
      <td><img src="'. $target_dir . "defaultPicture.png" .'" alt="lol" style="height:60px;width:60px;vertical-align:middle"> <a href="https://freedomaccount.000webhostapp.com/user.php?username='.$row['username'] . '">' .  $row['username'] . '</a></td>
           </tr>';
}
         
}

 } 
         
?>
</table>
<a href="/index.php" style="text-align: middle;">Home</a>
	</div>
	               	
</body>
</html>