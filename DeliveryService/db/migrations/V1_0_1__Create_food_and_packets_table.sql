CREATE TABLE IF NOT EXISTS food (
    id int not null auto_increment,
    name varchar(20),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS packets (
    id int not null auto_increment,
    food_id int,
    is_reserved bit,
    order_id binary(16),
    PRIMARY KEY (id)
);