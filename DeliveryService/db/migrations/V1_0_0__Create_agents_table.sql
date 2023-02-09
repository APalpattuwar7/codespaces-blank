CREATE TABLE IF NOT EXISTS agents (
    id int not null auto_increment,
    is_reserved bit,
    order_id binary(16),
    PRIMARY KEY (id)
);