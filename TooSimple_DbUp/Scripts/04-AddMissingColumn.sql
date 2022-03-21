alter table Goals 
add IsArchived boolean;

ALTER TABLE Goals RENAME COLUMN ExpenseFlag TO IsExpense;